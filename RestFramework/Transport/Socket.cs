using System;
using System.Net;
using System.Net.Sockets;
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

        public void StartListening(int incomingBuffer=1000)
        {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];

            // Create a TCP/IP socket.  
            m_listeningSocket = new SystemSocket(m_ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                m_listeningSocket.Bind(m_ipEndPoint);
                m_listeningSocket.Listen(incomingBuffer);

                ThreadPool.SetMaxThreads(40, 40);

                // Start listening for connections.  
                while (true)
                {
                    // Program is suspended while waiting for an incoming connection.  
                    SystemSocket handler = m_listeningSocket.Accept();

                    ThreadPool.QueueUserWorkItem(new HttpStreamReader().ListenSocketHandler, handler);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
    }
}
