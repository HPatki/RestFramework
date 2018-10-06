using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;

using System.Reflection;
using RestFramework.Helpers;
using RestFramework.Annotations;

using RestFramework.Factories;

namespace RestFramework.Container
{
    class HostContainer
    {
        private ServiceHost sh;

        public HostContainer(Type implementation)
        {
            sh = new ServiceHost(implementation);

            ServiceBehaviorAttribute servAttrib = sh.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            servAttrib.ConcurrencyMode = ConcurrencyMode.Single;
            servAttrib.InstanceContextMode = InstanceContextMode.PerCall;

            ServiceThrottlingBehavior throttleBehv = new ServiceThrottlingBehavior();
            throttleBehv.MaxConcurrentInstances = 100;
            throttleBehv.MaxConcurrentCalls = 100;

            sh.Description.Behaviors.Add(throttleBehv);

        }

        public void addService(Type contract, String address, Binding binding, IEndpointBehavior behavior)
        {
            ServiceEndpoint endPt = sh.AddServiceEndpoint(contract, binding, address);
            endPt.Behaviors.Add(behavior);

            endPt.Behaviors.Add(new MessageInterceptor());
        }

        public void StartService()
        {
            ControllerFactory factory = new ControllerFactory();
            factory.ConstructSingleTons();

            sh.Open();

            System.Console.WriteLine("The Service has been started ...");

            while (true) ;
        }
    }
}
