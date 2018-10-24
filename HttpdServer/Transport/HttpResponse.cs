using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace HttpdServer.Transport
{
    public class HttpResponse 
    {
        private Int32               m_StatusCode = 0;
        private String              m_StatusDesc;
        private HttpEntityHeader    m_EntityHeader;
        private HttpResponseHeader  m_ResponseHeader;
        private HttpBody            m_Body;

        public HttpResponse()
            //: base("HttpResponse")
        {
            //setType(typeof(HttpResponse));
            m_EntityHeader = new HttpEntityHeader();
            m_ResponseHeader = new HttpResponseHeader();
            m_Body = new HttpBody();
        }

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

        public String StatusDesc
        {
            get { return m_StatusDesc; }
            set { m_StatusDesc = value; }
        }

        public void AddResponseHeader(String name, Object val)
        {
            m_ResponseHeader.AddHeader(name, val);
        }

        public HttpBody Body
        {
            get { return m_Body; }
        }

        public byte[] Bytes()
        {   
            StringBuilder strBldr = new StringBuilder();
            strBldr.Append(m_StatusDesc+"\r\n");
            strBldr.Append("content-type: " + m_EntityHeader.ContentType + "\r\n");
            strBldr.Append("content-Length: " + m_EntityHeader.ContentLength + "\r\n");
            foreach (KeyValuePair<String, Object> kvPair in m_ResponseHeader)
            {
                strBldr.Append(kvPair.Key + " :" + kvPair.Value + "\r\n"); 
            }

            strBldr.Append("\r\n");

            byte[] msgheader = System.Text.Encoding.UTF8.GetBytes(strBldr.ToString());
            byte[] bodyBytes = m_Body.Body;
            byte[] arry = new byte[msgheader.Length + bodyBytes.Length];
            msgheader.CopyTo(arry, 0);
            bodyBytes.CopyTo(arry, msgheader.Length);
            
            return arry;
        }
    }
}
