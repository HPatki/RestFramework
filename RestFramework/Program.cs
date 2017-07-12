using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestFramework.Container;
using RestFramework.Broker;
using RestFramework.Interface;

using System.ServiceModel;
using System.ServiceModel.Description;

namespace RestFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            HostContainer hosted = new HostContainer(typeof(BrokerImpl));

            hosted.addService(typeof(IBroker), "http://localhost:16991/", new WebHttpBinding(), new WebHttpBehavior());

            hosted.StartService();
        }
    }
}
