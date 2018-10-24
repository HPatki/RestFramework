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
using HttpdServer.Transport;

using System.IO;

namespace HttpdServer
{
    public class Program
    {
        internal static int m_maxPayLoad;
        private static String def = "The HookFunction is not set. The Http Server will not do anything " + 
                " useful other than just parsing the request. Set the function using " + 
                " HttpStreamReader.HookFunc to make it do something interesting";
        private static Byte[] defByte = null;

        public static StreamWriter writer = new StreamWriter(new FileStream("F:\\tmp\\logger.txt", FileMode.Create));

        //public static AppSettings GetAppSettings () { return m_appSettings; }

        internal static byte[] DummyHookFunction(HttpRequest req)
        {
            if ( null == defByte)
                defByte = System.Text.Encoding.UTF8.GetBytes(def);

            HttpResponse response = new HttpResponse();
            response.StatusDesc = StatusCodeDesc.GetStatusDesc(200);
            response.ContentType = MediaTypeContent.GetContentType(MediaType.TEXT_PLAIN);
            response.ContentLength = (UInt64)defByte.Length;
            HttpBody body = response.Body;
            body.SetLengthOfBody(defByte.Length);
            body.addBodyContent(defByte, 0, defByte.Length);
            return response.Bytes(); 
        }

        public static void createServer()
        {
            m_maxPayLoad = 1024; //Int32.Parse(ConfigMgr.AppSettings["maxPayLoad"]);
            StatusCodeDesc.init();
            HttpdServer.Transport.Socket sock = new HttpdServer.Transport.Socket("127.0.0.1", 15990);
            sock.StartListening();
        }

    }
}
