using System;
using System.Net;
using System.Net.Http;

using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Sockets;
using SystemSocket =  System.Net.Sockets.Socket;
using HttpResp = RestFramework.Annotations.HttpResponse;

using RestFramework.Interface;
using HttpdServer.Transport;
using HttpdServer.Helpers;
using RestFramework.Annotations;
using RestFramework.Serde;
using RestFramework.Exceptions;

using System.Text.RegularExpressions;

namespace RestFramework.Broker
{
    sealed class BrokerImpl
    {
        private static BodyBinaryExtractor[] TransformBody(HttpRequest request)
        {
            List<BodyBinaryExtractor> BodyContent = new List<BodyBinaryExtractor>();

            String ContentType = request.GetHeaderValue("content-type");
            String ContentTypeUpper = null;
            if (null != ContentType && (ContentTypeUpper =
                ContentType.ToUpper()).Contains("MULTIPART/FORM-DATA"))
            {
                //Form could contain multiple fields
                Regex splitOnBoundary = new Regex("BOUNDARY");
                String[] splitContentType = splitOnBoundary.Split(ContentTypeUpper)[1].Split('='); ;
                Regex BodyContentSplitter = new Regex(splitContentType[1] + "\r\n");
                String[] parts = BodyContentSplitter.Split(System.Text.Encoding.UTF8.GetString(request.GetBody().Body));
                foreach (String part in parts)
                {
                    String partUpper = part.ToUpper();
                    if (partUpper.Contains("NAME"))
                    {
                        if (partUpper.Contains("CONTENT-TYPE")) //can mean a file
                        {
                            String fname = "", ftype = "", fcontent = "", pname="";
                            //name, file-name, file-type, content
                            Regex FileContent = new Regex("\r\n\r\n");
                            String[] FileContentPart = FileContent.Split(partUpper);
                            Regex Name = new Regex(" NAME=");
                            Regex FileName = new Regex("FILENAME=");
                            Regex FileType = new Regex("CONTENT-TYPE:");
                            Regex CRLF = new Regex("\r\n");
                            String[] NamePart = Name.Split(FileContentPart[0]);
                            if (NamePart.Length > 1)
                            {
                                String[] FileNamePart = FileName.Split(NamePart[1]);
                                if (FileNamePart.Length > 1)
                                {
                                    pname = FileNamePart[0].Split('"')[1];
                                    String[] FileTypePart = FileType.Split(FileNamePart[1]);
                                    fname = CRLF.Split(FileTypePart[0])[0].Split('"')[1];
                                    ftype = FileTypePart[1];
                                }
                            }

                            BodyFileExtractor bfe = new BodyFileExtractor();
                            bfe.FileName = fname;
                            bfe.FileType = ftype;
                            bfe.FileContent = System.Text.Encoding.UTF8.GetBytes(FileContentPart[1]);
                            bfe.ParamName = pname;

                            BodyContent.Add(bfe);
                        }
                        else //Binary
                        {
                            //name, content
                            Regex LineContent = new Regex("NAME=");
                            Regex CRLF = new Regex("\r\n");
                            String[] multi = LineContent.Split(partUpper);
                            BodyBinaryExtractor extracted = new BodyBinaryExtractor();
                            String[] pNameFContent = CRLF.Split(multi[1]);
                            extracted.FileContent = System.Text.Encoding.UTF8.GetBytes(pNameFContent[2]);
                            extracted.ParamName = pNameFContent[0].Split('"')[1];
                            BodyContent.Add(extracted);
                        }
                    }
                }
            }
            else
            {
                BodyBinaryExtractor extracted = new BodyBinaryExtractor();
                extracted.FileContent = request.GetBody().Body;
                extracted.ParamName = "";
                BodyContent.Add(extracted);
            }

            return BodyContent.ToArray();
        }

        public static Byte[] Process(HttpRequest Request)
        {
            //transform the body part
            BodyBinaryExtractor[] body = TransformBody(Request);

            Byte[] responseStream = null;
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
                     */
                    //Object retVal = mpr.DynamicInvoke(parameters);

                    retVal = mpr.GetMethodInfo().Invoke(mpr.GetObject(), parameters);

                }
                catch (UnsupportedMediaType err)
                {
                    response.StatusCode = 415;
                    response.ContentType = MediaTypeContent.GetContentType(MediaType.TEXT_PLAIN);
                    retVal = "Error during deserialising to JSON";
                }
                catch (Exception err)
                {
                    response.StatusCode = 500;
                    response.ContentType = MediaTypeContent.GetContentType(MediaType.TEXT_PLAIN);
                    retVal = "Internal Error " + err.Message;
                }
            }
            else
            {
                response.StatusCode = 404;
                response.ContentType = MediaTypeContent.GetContentType(MediaType.TEXT_PLAIN);
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
