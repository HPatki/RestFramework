using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text.RegularExpressions;
using System.Reflection;
using ConfigMgr = System.Configuration.ConfigurationManager;

using RestFramework.Container;
using RestFramework.Factories;
using RestFramework.Interface;
using RestFramework.Broker;

using HttpdServer.Helpers;
using HttpdServer.Transport;



namespace RestFramework
{
    public class Program
    {
        private static DelegateTypeFactory  m_delegateFactory;
        private static ControllerFactory    m_Controllers;

        //public static AppSettings GetAppSettings () { return m_appSettings; }

        public static void createFactories()
        {
            m_delegateFactory = new DelegateTypeFactory();
            m_Controllers = new ControllerFactory();
            m_Controllers.ConstructSingleTons();

            HttpdServer.Program.createServer(new BrokerImpl());
        }

        internal static DelegateTypeFactory getDelegateFactory ()
        {
            return m_delegateFactory;
        }

        internal static ControllerFactory getControllerFactory()
        {
            return m_Controllers;
        }

    }
}
