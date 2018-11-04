using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using SystemSocket = System.Net.Sockets.Socket;


namespace HttpdServer.Transport
{
    internal class Socket : IDisposable
    {
        private TCPSocket m_sock;
        private IPHostEntry                 m_ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        private IPAddress                   m_ipAddress;
        private IPEndPoint                  m_ipEndPoint;
        private int                         m_port;
        private String                      m_address;
        private SystemSocket                m_listeningSocket;

        public void Dispose()
        {
            m_listeningSocket.Dispose();
        }

        public Socket(String address, int port)
        {
            m_address = address;
            m_port = port;
            
            long ipadd;
            long.TryParse(address, out ipadd);

            m_ipAddress = new IPAddress(ipadd); // ipHostInfo.AddressList[0];
            m_ipEndPoint = new IPEndPoint(m_ipAddress, m_port);
        }

        public void StartListening(int incomingBuffer=1000)
        {
            String prt = m_port.ToString();
            SByte[] sprt = new SByte[prt.Length];
            for (int i = 0; i < prt.Length; ++i)
                sprt[i] = (sbyte)prt[i];

            unsafe
            {
                fixed (SByte* ppp = sprt)
                {
                    m_sock = new TCPSocket(ppp);
                }
            }

            ThreadPool.SetMaxThreads(40, 40);
            ThreadPool.SetMinThreads(40, 40);

            if (false == m_sock.InError())
                m_sock.StartListening();

            while (m_sock.Accept())
            {
                if (true == m_sock.InError())
                {
                    continue;
                }

                UInt32 handler = m_sock.ReturnClientSocket();

                var ThreadMain = System.Diagnostics.Stopwatch.StartNew();
                //ThreadPool.QueueUserWorkItem(Socket.StarterFunction, handler);
                ThreadPool.QueueUserWorkItem(HttpStreamReader.ListenSocketHandler, handler);
                ThreadMain.Stop();
            }
            
            // Create a TCP/IP socket.  
            //m_listeningSocket = new SystemSocket(m_ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            /*try
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
                    System.Console.WriteLine(handler.Handle);

                    var ThreadMain = System.Diagnostics.Stopwatch.StartNew();
                    //ThreadPool.QueueUserWorkItem(Socket.StarterFunction, handler);
                    ThreadPool.QueueUserWorkItem(HttpStreamReader.ListenSocketHandler, handler);
                    ThreadMain.Stop();
                    System.Console.WriteLine("Request Time " + ThreadMain.ElapsedMilliseconds);
                    
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
            }*/

        }
    }
}
