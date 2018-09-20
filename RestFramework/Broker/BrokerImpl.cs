using System;
using System.Net;
using System.Net.Http;

using System.Text;
using System.Threading.Tasks;

using RestFramework.Interface;
using RestFramework.Transport;
using RestFramework.Helpers;

namespace RestFramework.Broker
{
    sealed class BrokerImpl : IBroker
    {
        private HttpRequest m_Request;

        public BrokerImpl(HttpRequest request)
        {
            m_Request = request;
        }

        private String getURL ()
        {
            return m_Request.getRequestURI();
        }

        private String getQuery()
        {
            return "";
        }

        private Boolean getHeaders() 
        {
            return false;
        }

        public String Process()
        {
            Method mthd = m_Request.getMethod();

            switch (mthd)
            {
                case Method.GET:
                    Program.getControllerFactory().getMethodMapper(Method.GET, m_Request.getRequestURI());
                    break;
            }


            String message = "";
            
            return message;
        }
    }
}
