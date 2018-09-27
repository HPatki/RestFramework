using System;
using System.Net;
using System.Net.Http;

using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using RestFramework.Interface;
using RestFramework.Transport;
using RestFramework.Helpers;
using RestFramework.Annotations;

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
            ComponentMethodMapper mpr = null;
            String message = "";
            
            object[] ret = Program.getControllerFactory().getMethodMapper(mthd, m_Request.getRequestURI());
            mpr = ret[1] as ComponentMethodMapper;

            if (null != mpr)
            {
                object[] parameters = ExtractMethodParams.Extract(m_Request, mpr.getParamList());
                message = (String)mpr.DynamicInvoke(parameters);
            }
            else
            {
                //raise error
            }

            
            return message;
        }
    }
}
