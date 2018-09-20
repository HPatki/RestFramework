using System;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using SystemSocket = System.Net.Sockets.Socket;
using RestFramework.Interface;
using RestFramework.Broker;

namespace RestFramework.Transport
{
    class HttpStreamReader
    {
        private static Regex    m_parser = new Regex("\r\n\r\n");
        private HttpRequest     m_HttpRequest = new HttpRequest();
        
        public void ListenSocketHandler(Object state) //have to confirm to delegate signature
        {   
            int runCountOfBytesRecvd = 0;

            String data = null;
            byte[] bytes = new byte[1024];
            SystemSocket handler = (SystemSocket)state;
            // An incoming connection needs to be processed.  
            while (true)
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
                    m_HttpRequest.ConcatenateRawHeaderContent(splitted[0]);
                    int BodyLength = m_HttpRequest.ExtractHeaders().GetLengthOfBody();

                    if (1 < splitted.Length) //contains body part
                    {
                        var splittedByte = Encoding.UTF8.GetBytes(splitted[1]);
                        if (splittedByte.Length >= BodyLength)
                        {
                            ReadBody(0, handler, splittedByte);
                        }
                        else
                        {
                            ReadBody(BodyLength - splittedByte.Length, handler, splittedByte);
                        }

                        //hand off for further processing
                        var lBroker = new BrokerImpl(m_HttpRequest);
                        lBroker.Process();

                    }
               
                }
                else
                {
                    m_HttpRequest.ConcatenateRawHeaderContent(data);
                }
            }

            byte[] rr = System.Text.Encoding.ASCII.GetBytes("harshad suhas patki");
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
            strBldr.Append("harshad suhas patki");
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(strBldr.ToString());
            handler.Send(msg);
            handler.Close();
        }

        private void ReadBody(int BytesToRead, SystemSocket handler, byte[] BodyContent)
        {
            byte[] bytes = new byte[1024];
            int remainingBytes = BytesToRead;
            m_HttpRequest.ConcatenateBodyContent(BodyContent);
            while (remainingBytes > 0)
            {
                int bytesRec = handler.Receive(bytes);
                remainingBytes -= bytesRec;
                m_HttpRequest.ConcatenateBodyContent(bytes);
            }
        }
        
    }
}
