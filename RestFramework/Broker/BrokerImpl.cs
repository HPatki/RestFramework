using System;
using System.Net;
using System.Net.Http;

using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using SystemSocket =  System.Net.Sockets.Socket;

using RestFramework.Interface;
using RestFramework.Transport;
using RestFramework.Helpers;
using RestFramework.Annotations;

namespace RestFramework.Broker
{
    sealed class BrokerImpl : IBroker
    {
        private HttpRequest m_Request;
        private SystemSocket m_Handler;

        public BrokerImpl(HttpRequest request, SystemSocket handler)
        {
            m_Request = request;
            m_Handler = handler;
        }

        public void Process()
        {
            var response = new HttpResponse();

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
                response.StatusCode = 404;
            }

            m_Handler.Send(System.Text.Encoding.ASCII.GetBytes(response.ToString()));
            m_Handler.Close();

        }
    }
}
