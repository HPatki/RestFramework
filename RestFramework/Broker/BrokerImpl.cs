using System;
using System.Net;
using System.Net.Http;

using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Sockets;
using SystemSocket =  System.Net.Sockets.Socket;
using HttpResp = RestFramework.Annotations.HttpResponse;

using RestFramework.Interface;
using HttpdServer.Transport;
using HttpdServer.Helpers;
using RestFramework.Annotations;
using RestFramework.Serde;
using RestFramework.Exceptions;

namespace RestFramework.Broker
{
    sealed class BrokerImpl
    {
        public static Byte[] Process(HttpRequest Request)
        {
            Byte[] responseStream = null;
            var response = new HttpResp();
            var mthd = Request.getMethod();
            ComponentMethodMapper mpr = null;
            
            Object[] ret = Program.getControllerFactory().getMethodMapper(mthd, 
                                                            Request.getRequestURI());
            mpr = ret[1] as ComponentMethodMapper;
            Object retVal = null;
            
            if (null != mpr)
            {
                try
                {
                    Object[] parameters = ExtractMethodParams.Extract(Request, response,
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
            }
            else
            {
                response.StatusCode = 404;
                response.ContentType = MediaTypeContent.GetContentType(MediaType.TEXT_PLAIN);
                retVal = "Mapper for URL " + Request.getRequestURI() + " not found";
            }

            try
            {
                //marshall the response 
                //check if the response was passed to user code
                responseStream = MarshallOctet.marshall(response, retVal, mpr.Produces);
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

            return responseStream;
        }
    }
}
