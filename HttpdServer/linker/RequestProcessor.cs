using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using HttpdServer.Helpers;
using HttpdServer.Transport;

namespace HttpdServer.linker
{
    public class RequestProcessor
    {
        private static String def = "The HookFunction is not set. The Http Server will not do anything " +
                " useful other than just parsing the request. Set the function using " +
                " HttpStreamReader.HookFunc to make it do something interesting";

        private static Byte[] defByte = null;

        virtual public Byte[] Process(HttpRequest req)
        {
            return DummyHookFunction(req);
        }

        internal static byte[] DummyHookFunction(HttpRequest req)
        {
            if (null == defByte)
                defByte = System.Text.Encoding.UTF8.GetBytes(def);

            HttpResponse response = new HttpResponse();
            response.StatusDesc = StatusCodeDesc.GetStatusDesc(200);
            response.ContentType = MediaType.TEXT_PLAIN;
            response.ContentLength = (UInt64)defByte.Length;
            HttpBody body = response.Body;
            body.SetLengthOfBody(defByte.Length);
            body.SetBodyContent(defByte);
            return response.Bytes();
        }
    }
}
