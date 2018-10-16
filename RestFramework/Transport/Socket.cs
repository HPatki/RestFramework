using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using SystemSocket = System.Net.Sockets.Socket;

namespace RestFramework.Transport
{
    class Socket
    {
        private IPHostEntry                 m_ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        private IPAddress                   m_ipAddress;
        private IPEndPoint                  m_ipEndPoint;
        private int                         m_port;
        private String                      m_address;
        private SystemSocket                m_listeningSocket;

        public Socket(String address, int port)
        {
            m_address = address;
            m_port = port;
            
            long ipadd;
            long.TryParse(address, out ipadd);

            m_ipAddress = new IPAddress(ipadd); // ipHostInfo.AddressList[0];
            m_ipEndPoint = new IPEndPoint(m_ipAddress, m_port);
        }

        internal static void StarterFunction(Object dummy)
        {
            SystemSocket handler = (SystemSocket)dummy;
            String rr = "Dummy MEssage";
            System.Text.StringBuilder strBldr = new System.Text.StringBuilder();
            strBldr.Append("HTTP/1.1 200 OK\r\n");
            strBldr.Append("accept-ranges: bytes\r\n");
            strBldr.Append("vary: Accept-Encoding, Origin\r\n");
            strBldr.Append("content-encoding: gzip\r\n");
            strBldr.Append("content-type: text/plain; charset=UTF-8\r\n");
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
            handler.Disconnect(false);
        }

        public void StartListening(int incomingBuffer=1000)
        {
            // Create a TCP/IP socket.  
            m_listeningSocket = new SystemSocket(m_ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                m_listeningSocket.Bind(m_ipEndPoint);
                m_listeningSocket.Listen(incomingBuffer);

                ThreadPool.SetMaxThreads(40, 40);
                ThreadPool.SetMinThreads(40, 40);

                // Start listening for connections.  
                while (true)
                {
                    // Program is suspended while waiting for an incoming connection.  
                    SystemSocket handler = m_listeningSocket.Accept();

                    var ThreadMain = System.Diagnostics.Stopwatch.StartNew();
                    //ThreadPool.QueueUserWorkItem(Socket.StarterFunction, handler);
                    ThreadPool.QueueUserWorkItem(HttpStreamReader.ListenSocketHandler, handler);
                    ThreadMain.Stop();
                    System.Console.WriteLine("Thread Time " + ThreadMain.ElapsedMilliseconds);
                    
                    //log
                    //EndPoint endpt = handler.LocalEndPoint;
                    //long result = IPGlobalProperties.GetIPGlobalProperties()
                    //            .GetTcpIPv4Statistics()
                    //            .CurrentConnections;
                    //System.Console.WriteLine(result);
                    
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Environment.Exit(-1);
            }

        }
    }
}
