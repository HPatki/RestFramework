using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using SystemSocket = System.Net.Sockets.Socket;
using System.Collections.ObjectModel;


using RestFramework.Interface;
using RestFramework.Broker;

namespace RestFramework.Transport
{
    class HttpStreamReader
    {
        private static Regex    m_parser = new Regex("\r\n\r\n");
        
        public static void ListenSocketHandler(Object state) //have to confirm to delegate signature
        {
            var StartMain = System.Diagnostics.Stopwatch.StartNew();

            SystemSocket handler = (SystemSocket)state;
            HttpRequest  httpRequest = new HttpRequest();

            try
            {
                int runCountOfBytesRecvd = 0;

                String data = null, payload = null;
                byte[] bytes = new byte[8096];

                // An incoming connection needs to be processed.
                Boolean cont = true;
                while (cont)
                {
                    if (Program.m_maxPayLoad <= runCountOfBytesRecvd)
                        break; //error

                    int bytesRec = handler.Receive(bytes);
                    runCountOfBytesRecvd += bytesRec;
                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);

                    if (m_parser.IsMatch(data))
                    {
                        //we may have read past the header end. If so, we need to split at the exact 
                        //header end. Rest of the contents belong to body if any
                        var splitted = m_parser.Split(data);
                        httpRequest.ConcatenateRawHeaderContent(splitted[0]);

                        //httpRequest Body processing - START
                        Int32 headerBytes = System.Text.Encoding.UTF8.GetBytes(splitted[0]).Length + 4;
                        Int32 SkipSegments = 1;
                        int BodyLength = httpRequest.ExtractHeaders().GetLengthOfBody();
                        httpRequest.SetLengthOfBody(BodyLength);

                        String ContentType = httpRequest.GetHeaderValue("content-type");
                        if (null != ContentType  && ContentType.ToUpper().Contains("MULTIPART/FORM-DATA"))
                        {
                            SkipSegments += 1;
                            
                        }

                        if (false == splitted[1].Equals("")) //contains body part
                        {
                            Int32 len = 0;

                            if (2 == SkipSegments)
                            {
                                len = System.Text.Encoding.UTF8.GetBytes(splitted[1]).Length;
                                headerBytes += (len + 4);
                            }

                             ReadBody2(handler, httpRequest, bytes, headerBytes, bytesRec,
                                 BodyLength - len);

                        }
                        else //only header has been read so far
                        {
                            data = "";
                            runCountOfBytesRecvd = 0;

                            if (2 == SkipSegments) //multipart form-data
                            {
                                bool loop = true;
                                while (loop)
                                {
                                    bytesRec = handler.Receive(bytes);
                                    runCountOfBytesRecvd += bytesRec;
                                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                                    if (m_parser.IsMatch(data))
                                    {
                                        loop = false;
                                        splitted = m_parser.Split(data);
                                        headerBytes += System.Text.Encoding.UTF8.GetBytes(splitted[0]).Length + 4;

                                        ReadBody2(handler, httpRequest, bytes, headerBytes, bytesRec,
                                            BodyLength - System.Text.Encoding.UTF8.GetBytes(splitted[0]).Length);
                                    }
                                }
                            }
                            else
                            {
                                ReadBody(BodyLength, handler, new byte[0], httpRequest);
                            }

                        }

                        //hand off for further processing & final response
                        //this class is done with the request
                        var Start = System.Diagnostics.Stopwatch.StartNew();
                        var lBroker = new BrokerImpl(httpRequest, handler);
                        lBroker.Process();
                        Start.Stop();
                        System.Console.WriteLine("Time taken " + Start.ElapsedMilliseconds);
                        cont = false;
                    }
                    else
                    {
                        httpRequest.ConcatenateRawHeaderContent(data);
                    }
                }

                StartMain.Stop();
                System.Console.WriteLine("Main loop time " + StartMain.ElapsedMilliseconds);
            }
            catch (Exception err)
            {
                String rr = "Error occurred" + err.Message;
                StringBuilder strBldr = new StringBuilder();
                strBldr.Append("HTTP/1.1 200 OK\r\n");
                strBldr.Append("accept-ranges: bytes\r\n");
                strBldr.Append("vary: Accept-Encoding, Origin\r\n");
                strBldr.Append("content-encoding: gzip\r\n");
                strBldr.Append("content-type: text/javascript; charset=UTF-8\r\n");
                strBldr.Append("content-length:" + rr.Length * 2 + "\r\n");
                strBldr.Append("date: Fri, 15 Jun 2018 07:47:08 GMT\r\n");
                strBldr.Append("expires: Sat, 15 Jun 2019 07:47:08 GMT\r\n");
                strBldr.Append("last-modified: Wed, 13 Jun 2018 16:59:27 GMT\r\n");
                strBldr.Append("x-content-type-options: nosniff\r\n");
                strBldr.Append("server: sffe\r\n");
                strBldr.Append("x-xss-protection: 1; mode=block\r\n");
                strBldr.Append("cache-control: public, immutable, max-age=31536000\r\n");
                strBldr.Append("age: 51722\r\n");
                strBldr.Append("X-Firefox-Spdy: h2\r\n\r\n");
                strBldr.Append(rr);
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(strBldr.ToString());
                handler.Send(msg);
                handler.Close();
            }

            try
            {
                handler.Disconnect(false);
                //handler.Close(5);
            }
            catch (Exception err)
            {
                System.Console.WriteLine("error disconnecting the socket::" + err.Message);
                Environment.Exit(-1);
            }
        }

        private static void ReadBody2(SystemSocket handler, HttpRequest httpRequest,byte[] bytes,
                                      int headerBytes, int bytesRec,int PendingBodyLength 
                                      )
        {
            //BugFix-splitted will contain multiple parts all of which 
            //need to be considered to know how much of body has been
            //read. This was not happening
            List<Byte> Bldr = new List<Byte>();
            for (int i = headerBytes; i < bytesRec; ++i)
            {
                Bldr.Add(bytes[i]);
            }

            if (Bldr.Count >= PendingBodyLength)
            {
                ReadBody(0, handler, Bldr.ToArray(), httpRequest);
            }
            else
            {

                ReadBody(PendingBodyLength - Bldr.Count, handler, Bldr.ToArray(), httpRequest);
            }
        }

        private static void ReadBody(int BytesToRead, SystemSocket handler, byte[] BodyContent, HttpRequest httpRequest)
        {
            byte[] bytes = new byte[1024];
            int remainingBytes = BytesToRead;

            //can be multi-part form

            httpRequest.ConcatenateBodyContent(BodyContent,BodyContent.Length);

            while (remainingBytes > 0)
            {
                int bytesRec = handler.Receive(bytes);
                remainingBytes -= bytesRec;
                if (remainingBytes < 0)
                    System.Console.WriteLine("");
                httpRequest.ConcatenateBodyContent(bytes,bytesRec);
            }
        }
        
    }
}
