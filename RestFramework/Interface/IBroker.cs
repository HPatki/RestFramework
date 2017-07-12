using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using System.ServiceModel.Web;
using System.Net.Http;

using System.ServiceModel.Channels;

namespace RestFramework.Interface
{
    [ServiceContract]
    interface IBroker
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/*")]
        Message GenericGet1();
    }
}

