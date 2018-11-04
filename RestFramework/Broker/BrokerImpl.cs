using System;
using System.Net;
using System.Net.Http;

using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using SystemSocket =  System.Net.Sockets.Socket;
using HttpResp = RestFramework.Annotations.HttpResponse;

using RestFramework.Interface;
using HttpdServer.Transport;
using HttpdServer.Helpers;
using RestFramework.Annotations;
using RestFramework.Serde;
using RestFramework.Exceptions;
using HttpdServer.linker;

using System.Text.RegularExpressions;

namespace RestFramework.Broker
{
    sealed class BrokerImpl : RequestProcessor
    {
        private static Regex MultiFormPart = new Regex("multipart/form-data", RegexOptions.IgnoreCase);

        private static List<Int64> GetContentDispositionStarts(Byte[] bdy)
        {
            Int64 BodyLen = bdy.Length;
            List<Int64> markers = new List<long>();

            unsafe
            {
                fixed (byte* bdyPtr = bdy)
                {
                    for (int i = 0; i < BodyLen - 19; ) //-19 cos we check 19 positions in case of match
                    {
                        byte c = bdyPtr[i]; 
                        byte o = bdyPtr[i + 1];byte n = bdyPtr[i + 2];byte t = bdyPtr[i + 3];
                        byte e = bdyPtr[i + 4];byte n1 = bdyPtr[i + 5];byte t1 = bdyPtr[i + 6];
                        byte hyphen = bdyPtr[i + 7];byte d = bdyPtr[i + 8];byte i0 = bdyPtr[i + 9];
                        byte s = bdyPtr[i + 10];byte p = bdyPtr[i + 11];byte o1 = bdyPtr[i + 12];
                        byte s1 = bdyPtr[i + 13];byte i1 = bdyPtr[i + 14];byte t2 = bdyPtr[i + 15];
                        byte i2 = bdyPtr[i + 16];byte o2 = bdyPtr[i + 17];byte n2 = bdyPtr[i + 18];

                        if ((c == 67 || c == 99) /*c*/ && (o == 79 || o == 111) && /*o*/ 
                            (n == 78 || n == 110) && /*n*/
                            (t == 84 || t == 116) && /*t*/ (e == 69 || e == 101) && //e
                            (n1 == 78 || n1 == 110) && /*n*/ (t1 == 84 || t1 == 116) && /*t*/
                            (hyphen == 45) && /*-*/ (d == 68 || d == 100) && /*d*/
                            (i0 == 73 || i0 == 105) && /*i*/(s == 83 || s == 115) && /*s*/
                            (p == 80 || p == 112) && /*p*/(o1 == 79 || o1 == 111) && /*o*/
                            (s1 == 83 || s1 == 115) && /*s*/(i1 == 73 || i1 == 105) && /*i*/
                            (t2 == 84 || t2 == 116) && /*t*/(i2 == 73 || i2 == 105) && /*i*/
                            (o2 == 79 || o2 == 111) && /*o*/(n2 == 78 || n2 == 110)  /*n*/
                            )
                        {
                            markers.Add(i);
                            i += 19; //move ahead by length of 'content-disposition'

                            //identify the end of content-disposition. This is given by \r\n\r\n
                            for (int j = i; j < BodyLen - 3; ++j)
                            {
                                if ((bdyPtr[j] == 13) && (bdyPtr[j + 1] == 10) && (bdyPtr[j + 2] == 13) && (bdyPtr[j + 3] == 10))
                                {
                                    markers.Add(j);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            i += 1;
                        }
                    }
                }
            }

            return markers;
        }

        private static String GetName(Int64 Start, Int64 End, Byte[] bdy)
        {
            Int64 BodyLen = bdy.Length;
            List<Byte> Name = new List<byte>();
           
            String NameValue = null;

            unsafe
            {
                fixed (byte* bdyPtr = bdy)
                {
                    Boolean OpeningQuoteFound = false;
                    Boolean ClosingQuoteFound = false;
                    
                    for (long i = Start; i < End-3; ++i)
                    {
                        //name
                        if (  ((bdyPtr[i] == 78) || (bdyPtr[i] == 110)) &&      //n
                              ((bdyPtr[i+1] == 65) || (bdyPtr[i+1] == 97)) &&  //a
                              ((bdyPtr[i+2] == 77) || (bdyPtr[i+2] == 109)) && //m
                              ((bdyPtr[i+3] == 69) || (bdyPtr[i+3] == 101)) )   //e
                        {
                            if (bdyPtr[i - 1] != 32) //name is part of some other string
                                continue;

                            //skip =, "
                            Boolean EqualToFound = false;
                            
                            for (long skipper = i + 3; skipper < End; ++skipper)
                            {
                                if (bdyPtr[skipper] == 61)
                                {
                                    EqualToFound = true;
                                    continue;
                                }

                                if (EqualToFound && bdyPtr[skipper] == 34) //open double quote
                                {
                                    OpeningQuoteFound = true;
                                   
                                    for (long skipperEnd = skipper + 1; skipperEnd < End; ++skipperEnd)
                                    {
                                        if (bdyPtr[skipperEnd] == 34) //close double quote
                                        {
                                            ClosingQuoteFound = true;
                                            break;
                                        }
                                        else
                                        {
                                            Name.Add(bdyPtr[skipperEnd]);
                                        }
                                    }

                                    break;
                                }

                            }

                            break;
                        }
                    }

                    if (OpeningQuoteFound && ClosingQuoteFound)
                        NameValue = System.Text.Encoding.ASCII.GetString(Name.ToArray()); 
                }
            }

            return NameValue;
        }

        private static String GetContentType(Int64 Start, Int64 End, Byte[] bdy)
        {
            Int64 BodyLen = bdy.Length;
            List<Byte> Name = new List<byte>();
            
            String NameValue = null;

            unsafe
            {
                fixed (byte* bdyPtr = bdy)
                {
                    Boolean SpaceBeforeType = false;
                    Boolean SpaceAfterType = false;

                    for (long i = Start; i < End-10; ++i)
                    {
                        //content-type
                        if (((bdyPtr[i] == 67) || (bdyPtr[i] == 99)) &&      //c
                              ((bdyPtr[i + 1] == 79) || (bdyPtr[i + 1] == 111)) &&  //o
                              ((bdyPtr[i + 2] == 78) || (bdyPtr[i + 2] == 110)) && //n
                              ((bdyPtr[i + 3] == 84) || (bdyPtr[i + 3] == 116)) && //t
                              ((bdyPtr[i + 4] == 69) || (bdyPtr[i + 4] == 101)) && //e
                              ((bdyPtr[i + 5] == 78) || (bdyPtr[i + 5] == 110)) && //n
                              ((bdyPtr[i + 6] == 84) || (bdyPtr[i + 6] == 116)) && //t
                              ((bdyPtr[i + 7] == 45) ) && //-
                              ((bdyPtr[i + 8] == 84) || (bdyPtr[i + 8] == 116)) && //t
                              ((bdyPtr[i + 9] == 89) || (bdyPtr[i + 9] == 121)) && //y
                              ((bdyPtr[i + 10] == 80) || (bdyPtr[i + 10] == 112)) && //p
                              ((bdyPtr[i + 11] == 69) || (bdyPtr[i + 11] == 101)) //e
                            )   //t
                        {
                            //skip :
                            Boolean ColonFound = false;

                            for (long skipper = i + 12; skipper < End; ++skipper)
                            {
                                if (bdyPtr[skipper] == 58)
                                {
                                    ColonFound = true;
                                    continue;
                                }

                                if (ColonFound && bdyPtr[skipper] == 32) //space before 
                                {
                                    SpaceBeforeType = true;
                                    continue;
                                }

                                if (ColonFound)
                                {
                                    for (long skipperEnd = skipper; skipperEnd < End; ++skipperEnd)
                                    {
                                        if (bdyPtr[skipperEnd] == 32) //space after
                                        {
                                            SpaceAfterType = true;
                                            break;
                                        }
                                        else
                                        {
                                            Name.Add(bdyPtr[skipperEnd]);
                                        }
                                    }

                                    SpaceAfterType = true; //since there are no trailing spaces
                                }

                                break;

                            }

                            break;
                        }
                    }

                    if (SpaceBeforeType && SpaceAfterType)
                        NameValue = System.Text.Encoding.ASCII.GetString(Name.ToArray());
                }
            }

            return NameValue;
        }

        private static String GetFileName(Int64 Start, Int64 End, Byte[] bdy)
        {
            Int64 BodyLen = bdy.Length;
            List<Byte> Name = new List<byte>();

            String NameValue = null;

            unsafe
            {
                fixed (byte* bdyPtr = bdy)
                {
                    Boolean OpeningQuoteFound = false;
                    Boolean ClosingQuoteFound = false;

                    for (long i = Start; i < End-6; ++i)
                    {
                        //filename
                        if (((bdyPtr[i] == 70) || (bdyPtr[i] == 102)) &&      //f
                              ((bdyPtr[i + 1] == 73) || (bdyPtr[i + 1] == 105)) &&  //i
                              ((bdyPtr[i + 2] == 76) || (bdyPtr[i + 2] == 108)) && //l
                              ((bdyPtr[i + 3] == 69) || (bdyPtr[i + 3] == 101)) && //e
                              ((bdyPtr[i + 4] == 78) || (bdyPtr[i + 4] == 110)) && //n
                              ((bdyPtr[i + 5] == 65) || (bdyPtr[i + 5] == 97)) && //a
                              ((bdyPtr[i + 6] == 77) || (bdyPtr[i + 6] == 109)) && //m
                              ((bdyPtr[i + 7] == 69) || (bdyPtr[i + 7] == 101))  //e
                            ) 
                        {
                            //skip = "
                            Boolean EqualToFound = false;

                            for (long skipper = i + 7; skipper < End; ++skipper)
                            {
                                if (bdyPtr[skipper] == 61)
                                {
                                    EqualToFound = true;
                                    continue;
                                }

                                if (EqualToFound && bdyPtr[skipper] == 34) //opening quote
                                {
                                    OpeningQuoteFound = true;
                                    continue;
                                }

                                if (OpeningQuoteFound)
                                {
                                    for (long skipperEnd = skipper; skipperEnd < End; ++skipperEnd)
                                    {
                                        if (bdyPtr[skipperEnd] == 34) //closing quote
                                        {
                                            ClosingQuoteFound = true;
                                            break;
                                        }
                                        else
                                        {
                                            Name.Add(bdyPtr[skipperEnd]);
                                        }
                                    }

                                    break;
                                }

                            }

                            break;
                        }
                    }

                    if (OpeningQuoteFound && ClosingQuoteFound)
                        NameValue = System.Text.Encoding.ASCII.GetString(Name.ToArray());
                }
            }

            return NameValue;
        }

        private static String GetNameValue(Int64 Start, Int64 End, Byte[] bdy)
        {
            //read upto next /r/n
           
            return null;
        }

        private static BodyBinaryExtractor[] TransformBody(HttpRequest request)
        {
            List<BodyBinaryExtractor> BodyContent = new List<BodyBinaryExtractor>();
            
            String ContentType = request.GetHeaderValue("content-type");
           
            if (null != ContentType && MultiFormPart.IsMatch(ContentType)) //Form-data
            {
                String[] Boundary = ContentType.Split('=');
                Byte[] bdy = request.GetBody().Body;
                
                System.Diagnostics.Stopwatch forLoop = System.Diagnostics.Stopwatch.StartNew();

                //content-disposition starts & Ends
                List<Int64> cdMarkers = GetContentDispositionStarts(bdy);
                
                forLoop.Stop();
                //System.Console.WriteLine ("Time for matches :: " + forLoop.ElapsedMilliseconds);

                //System.Console.WriteLine("CDMarker Size is " + cdMarkers.Count);

                if (cdMarkers.Count > 8)
                {
                    //System.Console.WriteLine("");
                }

                for (int cntr = 0; cntr < cdMarkers.Count; cntr += 2 )
                {
                    //System.Console.WriteLine("Count is " + cntr);
                    //System.Console.WriteLine("List elements " + BodyContent.Count);
                    //GetName
                    //check if content-type present, if yes get the content type
                    //and the file name
                    String Name = GetName(cdMarkers[cntr], cdMarkers[cntr + 1], bdy);
                    String ContType = GetContentType(cdMarkers[cntr], cdMarkers[cntr + 1], bdy);
                    String FileName = GetFileName(cdMarkers[cntr], cdMarkers[cntr + 1], bdy);
                    String NameValue = GetNameValue(cdMarkers[cntr], cdMarkers[cntr + 1], bdy);
                    BodyBinaryExtractor extracted = null;
                    if (null != ContType) //file
                    {
                        extracted = new BodyFileExtractor();
                        var BDFEX = extracted as BodyFileExtractor;
                        BDFEX.FileName = FileName;
                        BDFEX.FileType = ContType;
                    }
                    else
                    {
                        extracted = new BodyBinaryExtractor();
                    }

                    extracted.FileContent = request.GetBody().Body;
                    extracted.ParamName = Name;
                    extracted.StartPos = cdMarkers[cntr + 1] + 4;
                    if (cntr != cdMarkers.Count - 2)
                        extracted.EndPos = cdMarkers[cntr + 2] - (7 + Boundary[1].Length); //2 for --, 4 for CRLF
                    else
                        extracted.EndPos = bdy.Length - (7 + Boundary[1].Length);
                    BodyContent.Add(extracted);
                }
            }
            else
            {
                BodyBinaryExtractor extracted = new BodyBinaryExtractor();
                extracted.FileContent = request.GetBody().Body;
                extracted.ParamName = "";
                extracted.StartPos = 0;
                extracted.EndPos = request.GetLengthOfBody();
                BodyContent.Add(extracted);
            }

            return BodyContent.ToArray();
        }

        override public Byte[] Process(HttpRequest Request)
        {
            //transform the body part
            BodyBinaryExtractor[] body = TransformBody(Request);

            byte[] responseStream = null;
            var response = new HttpResp();
            var mthd = Request.getMethod();
            
            Object[] ret = Program.getControllerFactory().getMethodMapper(mthd, 
                                                            Request.getRequestURI());
            var mpr = ret[1] as ComponentMethodMapper;
            Object retVal = null;
            
            if (null != mpr)
            {
                try
                {
                    Object[] parameters = ExtractMethodParams.Extract(Request, body, response,
                        mpr.getParamList(), mpr.Consumes);

                    /* Invoke using MethodInfo is way faster than a delegate
                     *
                     * Object retVal = mpr.DynamicInvoke(parameters);
                     */

                    retVal = mpr.GetMethodInfo().Invoke(mpr.GetObject(), parameters);

                }
                catch (UnsupportedMediaType err)
                {
                    response.StatusCode = 415;
                    response.ContentType = MediaType.TEXT_PLAIN;
                    retVal = "Error during deserialising to JSON";
                }
                catch (Exception err)
                {
                    response.StatusCode = 500;
                    response.ContentType = MediaType.TEXT_PLAIN;
                    retVal = "Internal Error " + err.Message;
                }
            }
            else
            {
                response.StatusCode = 404;
                response.ContentType = MediaType.TEXT_PLAIN;
                retVal = "Mapper for URL " + Request.getRequestURI() + " not found";
            }

            try
            {
                //marshall the response 
                //check if the response was passed to user code
                responseStream = MarshallOctet.marshall(response, retVal, mpr.Produces);
            }
            catch (ArgumentNullException err)
            {
                //TODO
            }
            catch (SocketException err)
            {
                //TODO
            }
            catch (ObjectDisposedException err)
            {
                //TODO
            }
            catch (Exception err)
            {
                //TODO
            }

            return responseStream;
        }
    }
}
