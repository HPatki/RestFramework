using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Web;

using RestFramework.Interface;

namespace RestFramework.Broker
{
    class BrokerImpl : IBroker
    {
        public Message GenericGet1()
        {
            String message = "{\"Name\":\"Harshad\"}";
            return WebOperationContext.Current.CreateJsonResponse<String> (message);
        }
    }
}
