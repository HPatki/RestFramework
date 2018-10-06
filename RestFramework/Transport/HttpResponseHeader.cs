using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestFramework.Transport
{
    class HttpResponseHeader
    {
        Dictionary<String, Object> m_headers;

        internal void AddHeader(String name, Object val)
        {
            m_headers.Add(name, val);
        }
    }
}
