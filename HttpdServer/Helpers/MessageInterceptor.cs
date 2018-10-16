using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;

using System.IO;

namespace HttpdServer.Helpers
{
    public class MessageInterceptor : IDispatchMessageInspector, IEndpointBehavior
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            String result = request.GetBodyAttribute("searchParams", "");
            System.Console.WriteLine("Intercepted !!");

            return request;

        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            System.Console.WriteLine("Going Back!!");
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {

        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }
    }
}
