using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Net.Sockets;
using SystemSocket = System.Net.Sockets.Socket;
using System.Collections.ObjectModel;


namespace HttpdServer.Transport
{
    public class HttpStreamReader
    {
        private static Regex    m_parser = new Regex("\r\n\r\n");
        public delegate byte[] HookFunction (HttpRequest request);

        //this is set by the main program and not the caller
        private static HookFunction m_fn = Program.DummyHookFunction; 

        private static byte[] CallHook (HttpRequest req)
        {
            return m_fn(req);
        }

        public static HookFunction HookFunc
        {
            get { return m_fn; }
            set { m_fn = value; }
        }
        
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
                Boolean BodyProcessed = false;
                while (false == BodyProcessed)
                {
                    if (Program.m_maxPayLoad <= runCountOfBytesRecvd)
                        break; //error

                    int bytesRec = handler.Receive(bytes);
                    
                    Program.writer.WriteLine(System.Text.Encoding.UTF8.GetString(bytes));
                    
                    runCountOfBytesRecvd += bytesRec;
                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);

                    if (m_parser.IsMatch(data))
                    {
                        //we may have read past the header end. If so, we need to split at the exact 
                        //header end. Rest of the contents belong to body if any
                        var splitted = m_parser.Split(data);
                        httpRequest.ConcatenateRawHeaderContent(splitted[0]);
                        int BodyLength = httpRequest.ExtractHeaders().GetLengthOfBody();
                        Int32 headerBytes = System.Text.Encoding.UTF8.GetBytes(splitted[0]).Length + 4;            
                        //httpRequest Body processing - START
                        BodyProcessed = true;
                        ExtractBody(bytes, bytesRec, handler, httpRequest, headerBytes, BodyLength);
                        
                    }
                    else
                    {
                        httpRequest.ConcatenateRawHeaderContent(data);
                    }
                }

                Byte[] ret = CallHook(httpRequest);
                int sent = 0, start =0, remaining = ret.Length ;
                do
                {
                    sent = handler.Send(ret, start, remaining, System.Net.Sockets.SocketFlags.None);
                    if (sent < remaining)
                    {
                        start = sent;
                        remaining = remaining - sent;
                    }
                    else
                    {
                        remaining = remaining - sent;
                    }

                } while (remaining > 0);
                
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
            }
            finally
            {
                try
                {
                    //wait till all the data is sent
                    LingerOption linger = new LingerOption(true, 5); 
                    handler.LingerState = linger;
                    handler.Disconnect(false);
                    //handler = null;
                    //handler.Close(5);
                }
                catch (Exception err)
                {
                    System.Console.WriteLine("error disconnecting the socket::" + err.Message);
                    Environment.Exit(-1);
                }
            }
        }

        private static void ExtractBody(Byte[] bytes, Int32 BodyBytesInHeader, SystemSocket Handler, HttpRequest httpRequest,
                                 Int32 HeaderBytesToSkip,
                                 Int32 BodyLength)
        {
            httpRequest.SetLengthOfBody(BodyLength);

            //read in the complete body first
            ReadBody(Handler, httpRequest, bytes, BodyBytesInHeader, BodyLength, HeaderBytesToSkip);
        }

        private static void ReadBody(SystemSocket handler, HttpRequest httpRequest,
                                byte[] BodyContent, Int32 BodyBytesInHeader, Int32 BodyLength, Int32 HeaderBytesToSkip)
        {
            var stpwtch = System.Diagnostics.Stopwatch.StartNew();

            if (0 == BodyLength)
                return;
            
            int bytesRec = 0, readSoFar = 0;
            
            List<Byte> BByte = new List<Byte>(BodyLength);
            for (int i = HeaderBytesToSkip; i < BodyBytesInHeader; ++i)
            {
                ++readSoFar;
                BByte.Add(BodyContent[i]);
            }

            Int32 remaining = BodyLength - readSoFar;
            HttpBody body = httpRequest.GetBody();

            if (readSoFar < BodyLength)
            {
                //body contents remain to be read completely
                do
                {
                    byte[] bytes = new byte[1024];
                    bytesRec = handler.Receive(bytes);
                    BByte.AddRange(bytes);
                    readSoFar += bytesRec;
                    remaining -= bytesRec;
                    /*foreach (Byte b in bytes)
                    {
                        BByte.AddRange(bytes);
                    }*/

                } while (remaining != 0);
            }

            body.Body = BByte.ToArray();

            stpwtch.Stop();
            System.Console.WriteLine("Time in Copying Body :: " + stpwtch.ElapsedMilliseconds);
            System.Console.WriteLine("Body Length :: " + BodyLength);
            System.Console.WriteLine("Total Bytes Read :: " + readSoFar);
        }

        public static byte[] DefaultHookFunc (HttpRequest request)
        {
            return null;
        }
        
    }
}
