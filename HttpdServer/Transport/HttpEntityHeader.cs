using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpdServer.Transport
{
    public class HttpEntityHeader
    {
        private String m_ContentEncoding = "";
        private UInt64 m_ContentLength = 0;
        private String m_ContentType = "";

        internal String ContentEncoding
        {
            get { return m_ContentEncoding; }
            set { m_ContentEncoding = value; }
        }

        internal UInt64 ContentLength
        {
            get { return m_ContentLength; }
            set { m_ContentLength = value; }
        }

        internal String ContentType
        {
            get { return m_ContentType; }
            set { m_ContentType = value; }
        }
    }
}
