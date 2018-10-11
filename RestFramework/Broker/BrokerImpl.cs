using System;
using System.Net;
using System.Net.Http;

using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Sockets;
using SystemSocket =  System.Net.Sockets.Socket;

using RestFramework.Interface;
using RestFramework.Transport;
using RestFramework.Helpers;
using RestFramework.Annotations;
using RestFramework.Serde;

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
            var mthd = m_Request.getMethod();
            ComponentMethodMapper mpr = null;
            
            Object[] ret = Program.getControllerFactory().getMethodMapper(mthd, m_Request.getRequestURI());
            mpr = ret[1] as ComponentMethodMapper;

            if (null != mpr)
            {
                Object[] parameters = ExtractMethodParams.Extract(m_Request, response, 
                    mpr.getParamList(), mpr.Consumes);

                /* Invoke using MethodInfo is way faster than a delegate
                 */
                //Object retVal = mpr.DynamicInvoke(parameters);

                Object retVal = mpr.GetMethodInfo().Invoke(mpr.GetObject(), parameters);
                
                //marshall the response 
                //check if the response was passed to user code
                Boolean UsrCodeHandledResponse = false;
                foreach (Object paramobj in parameters)
                {
                    if (paramobj == response)
                        UsrCodeHandledResponse = true;
                }

                Byte[] responseStream = MarshallOctet.marshall(response, retVal, 
                                        UsrCodeHandledResponse, mpr.Produces);

                m_Handler.Send(responseStream);

            }
            else
            {
                response.StatusCode = 200;
            }

            //System.Console.WriteLine (m_Handler.Send(response.Bytes()));
            //m_Handler.LingerState = new LingerOption(true,2);
            //m_Handler.Disconnect(false);

        }
    }
}
