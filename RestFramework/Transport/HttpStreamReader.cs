﻿using System;
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
        
        public void ListenSocketHandler(Object state) //have to confirm to delegate signature
        {
            SystemSocket handler = (SystemSocket)state;
            HttpRequest  httpRequest = new HttpRequest();

            try
            {
                int runCountOfBytesRecvd = 0;

                String data = null, payload = null;
                byte[] bytes = new byte[1024];

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
                        int BodyLength = httpRequest.ExtractHeaders().GetLengthOfBody();

                        if (1 < splitted.Length) //contains body part
                        {
                            //BugFix-splitted will contain multiple parts all of which 
                            //need to be considered to know how much of body has been
                            //read. This was not happening
                            StringBuilder Bldr = new StringBuilder(1024);
                            int skip = 1;
                            foreach (String oneStr in splitted)
                            {
                                if (0 < skip--)
                                    continue;
                                Bldr.Append(oneStr);
                            }
                            var splittedByte = Encoding.UTF8.GetBytes(Bldr.ToString());

                            if (splittedByte.Length >= BodyLength)
                            {
                                ReadBody(0, handler, splittedByte, httpRequest);
                            }
                            else
                            {
                                ReadBody(BodyLength - splittedByte.Length, handler, splittedByte, httpRequest);
                            }

                        }
                        else
                        {
                            ReadBody(BodyLength, handler, new byte[0], httpRequest);
                        }

                        //hand off for further processing & final response
                        //this class is done with the request
                        var lBroker = new BrokerImpl(httpRequest, handler);
                        lBroker.Process();
                        cont = false;
                    }
                    else
                    {
                        httpRequest.ConcatenateRawHeaderContent(data);
                    }
                }

                /*byte[] rr = System.Text.Encoding.ASCII.GetBytes("harshad suhas patki");
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
                strBldr.Append(payload);
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(strBldr.ToString());
                handler.Send(msg);
                handler.Close();*/
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

        private void ReadBody(int BytesToRead, SystemSocket handler, byte[] BodyContent, HttpRequest httpRequest)
        {
            byte[] bytes = new byte[1024];
            int remainingBytes = BytesToRead;
            httpRequest.ConcatenateBodyContent(BodyContent);
            while (remainingBytes > 0)
            {
                int bytesRec = handler.Receive(bytes);
                remainingBytes -= bytesRec;
                httpRequest.ConcatenateBodyContent(bytes);
            }
        }
        
    }
}
