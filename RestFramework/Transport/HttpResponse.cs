using System;
using System.Collections.Generic;
using System.Text;

namespace RestFramework.Transport
{
    class HttpResponse
    {
        private Int32               m_StatusCode;
        private HttpEntityHeader    m_EntityHeader;
        private HttpResponseHeader  m_ResponseHeader;

        public String ContentEncoding
        {
            get { return m_EntityHeader.ContentEncoding; }
            set { m_EntityHeader.ContentEncoding = value; }
        }

        public UInt64 ContentLength
        {
            get { return m_EntityHeader.ContentLength; }
            set { m_EntityHeader.ContentLength = value; }
        }

        public String ContentType
        {
            get { return m_EntityHeader.ContentType; }
            set { m_EntityHeader.ContentType = value; }
        }

        public Int32 StatusCode
        {
            get { return m_StatusCode; }
            set { m_StatusCode = value; }
        }

        public override string ToString()
        {
            byte[] body = System.Text.Encoding.ASCII.GetBytes("URL Not Found");
            StringBuilder strBldr = new StringBuilder();
            strBldr.Append("HTTP/1.1 " + StatusCode + " Bad Gateway\r\n");
            strBldr.Append("content-type: application/octet-stream; charset=UTF-8\r\n");
            strBldr.Append("content-length:" + body.Length + "\r\n");
            strBldr.Append("\r\n");
            foreach (byte b in body)
                strBldr.Append(b + " ");
            return strBldr.ToString();
        }
    }
}
