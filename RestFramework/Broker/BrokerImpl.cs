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
using RestFramework.Exceptions;

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
            
            Object[] ret = Program.getControllerFactory().getMethodMapper(mthd, 
                                                            m_Request.getRequestURI());
            mpr = ret[1] as ComponentMethodMapper;
            Object retVal = null;
            
            if (null != mpr)
            {
                try
                {
                    Object[] parameters = ExtractMethodParams.Extract(m_Request, response,
                        mpr.getParamList(), mpr.Consumes);

                    /* Invoke using MethodInfo is way faster than a delegate
                     */
                    //Object retVal = mpr.DynamicInvoke(parameters);

                    retVal = mpr.GetMethodInfo().Invoke(mpr.GetObject(), parameters);

                }
                catch (UnsupportedMediaType err)
                {
                    response.StatusCode = 415;
                    response.ContentType = MediaTypeContent.GetContentType(MediaType.TEXT_PLAIN);
                    retVal = "Error during deserialising to JSON";
                }
                catch (Exception err)
                {
                    response.StatusCode = 500;
                    response.ContentType = MediaTypeContent.GetContentType(MediaType.TEXT_PLAIN);
                    retVal = "Internal Error " + err.Message;
                }
                finally
                {
                    try
                    {
                        //marshall the response 
                        //check if the response was passed to user code
                        Byte[] responseStream = MarshallOctet.marshall(response, retVal, mpr.Produces);

                        m_Handler.Send(responseStream);
                    }
                    catch (ArgumentNullException err)
                    {
                        //TODO
                    }
                    catch (SocketException err)
                    {
                        //TODO
                    }
                    catch (ObjectDisposedException err)
                    {
                        //TODO
                    }
                    catch (Exception err)
                    {
                        //TODO
                    }
                }

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
