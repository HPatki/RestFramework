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
using HttpdServer.Helpers;

namespace HttpdServer
{
    public class Program
    {
        internal static int m_maxPayLoad;
       
        //public static AppSettings GetAppSettings () { return m_appSettings; }

        public static void createServer()
        {
            m_maxPayLoad = 1024; //Int32.Parse(ConfigMgr.AppSettings["maxPayLoad"]);
            StatusCodeDesc.init();
            HttpdServer.Transport.Socket sock = new HttpdServer.Transport.Socket("127.0.0.1", 15990);
            sock.StartListening();
        }

        //static void Main(string[] args)
        //{
        //    m_maxPayLoad = Int32.Parse(ConfigMgr.AppSettings["maxPayLoad"]);
        //    createFactories();

        //    RestFramework.Transport.Socket sock = new Transport.Socket("127.0.0.1",15990);
        //    sock.StartListening();

            //MethodInfo info = typeof(HostContainer).GetMethod("addService");
            //ParameterInfo [] paramss = info.GetParameters();
            //RuntimeParameterInfo
            //Object[] methodArgs = new Object[paramss.Length];
            //for (int i = 0; i < paramss.Length; ++i)
            //{
            //    methodArgs[i] = Convert.ChangeType(null, paramss[i].ParameterType);
            //}
            
            //HostContainer hosted = new HostContainer(typeof(BrokerImpl));
            //info.Invoke(hosted, methodArgs);

            //hosted.addService(typeof(IBroker), , new WebHttpBinding(), new WebHttpBehavior());

            //hosted.StartService();
        //}
    }
}
