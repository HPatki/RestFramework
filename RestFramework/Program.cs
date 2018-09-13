using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestFramework.Container;
using RestFramework.Broker;
using RestFramework.Interface;
using RestFramework.Helpers;

using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text.RegularExpressions;
using System.Reflection;
using ConfigMgr = System.Configuration.ConfigurationManager;

namespace RestFramework
{
    class Program
    {
        internal static int m_maxPayLoad;
        private static DelegateTypeFactory  m_delegateFactory;
        private static ControllerFactory    m_Controllers;

        //public static AppSettings GetAppSettings () { return m_appSettings; }
        public static DelegateTypeFactory getDelegateFactory ()
        {
            return m_delegateFactory;
        }

        static void Main(string[] args)
        {
            m_maxPayLoad = Int32.Parse(ConfigMgr.AppSettings["maxPayLoad"]);
            m_delegateFactory = new DelegateTypeFactory();
            m_Controllers = new ControllerFactory();
            m_Controllers.ConstructSingleTons();

            RestFramework.Transport.Socket sock = new Transport.Socket("127.0.0.1",15990);
            sock.StartListening();

            MethodInfo info = typeof(HostContainer).GetMethod("addService");
            ParameterInfo [] paramss = info.GetParameters();
            //RuntimeParameterInfo
            Object[] methodArgs = new Object[paramss.Length];
            for (int i = 0; i < paramss.Length; ++i)
            {
                methodArgs[i] = Convert.ChangeType(null, paramss[i].ParameterType);
            }
            
           HostContainer hosted = new HostContainer(typeof(BrokerImpl));
           //info.Invoke(hosted, methodArgs);

            hosted.addService(typeof(IBroker), "http://192.168.0.25", new WebHttpBinding(), new WebHttpBehavior());

            hosted.StartService();
        }
    }
}
