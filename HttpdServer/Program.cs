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

using System.IO;

using HttpdServer.Helpers;
using HttpdServer.linker;

namespace HttpdServer
{
    public class Program
    {
        internal static int m_maxPayLoad;
        private static RequestProcessor processor;
 
        //public static AppSettings GetAppSettings () { return m_appSettings; }

        public static void createServer(RequestProcessor processor)
        {
            if (null == processor)
            {
                Program.processor = new RequestProcessor();
            }
            else
            {
                Program.processor = processor;
            }

            m_maxPayLoad = 1024; //Int32.Parse(ConfigMgr.AppSettings["maxPayLoad"]);
            StatusCodeDesc.init();
            HttpdServer.Transport.Socket sock = new HttpdServer.Transport.Socket("127.0.0.1", 15990);
            sock.StartListening();
        }

        public static RequestProcessor Processor
        {
            get { return processor; }
        }

    }
}
