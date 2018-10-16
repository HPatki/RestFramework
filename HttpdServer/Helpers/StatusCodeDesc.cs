using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpdServer.Helpers
{
    public class StatusCodeDesc
    {
        internal readonly static Dictionary<Int32, String> m_CodeToDesc = new Dictionary<int,string>();

        public static void init()
        {
            m_CodeToDesc.Add(200, "HTTP/1.1 200 OK");
            m_CodeToDesc.Add(400, "HTTP/1.1 400 Bad Request");
            m_CodeToDesc.Add(401, "HTTP/1.1 401 Unauthorized");
            m_CodeToDesc.Add(403, "HTTP/1.1 403 Forbidden");
            m_CodeToDesc.Add(404, "HTTP/1.1 404 Not Found");
            m_CodeToDesc.Add(415, "HTTP/1.1 415 Unsupported Media Type");
            
        }

        public static String GetStatusDesc(Int32 code)
        {
            String str;
            m_CodeToDesc.TryGetValue(code, out str);
            return str;
        }

         
    }
}
