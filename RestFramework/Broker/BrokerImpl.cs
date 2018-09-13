using System;
using System.Net;
using System.Net.Http;

using System.Text;
using System.Threading.Tasks;

using RestFramework.Interface;
using RestFramework.Transport;

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

        public String GenericGet()
        {
            String message = "";

            return message;
        }

        public String GenericPost()
        {
            String message = "";
            
            return message;
        }

        public String GenericPut()
        {
            String message = "";
           
            return message;
        }

        public String GenericDelete()
        {
            String message = "";
            
            return message;
        }
    }
}
