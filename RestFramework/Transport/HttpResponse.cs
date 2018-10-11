using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using RestFramework.Annotations;

namespace RestFramework.Transport
{
    public class HttpResponse : BaseAttribute
    {
        private Int32               m_StatusCode = 0;
        private String              m_StatusDesc;
        private HttpEntityHeader    m_EntityHeader;
        private HttpResponseHeader  m_ResponseHeader;
        private Byte[]              m_Body;

        public HttpResponse()
            : base("HttpResponse")
        {
            setType(typeof(HttpResponse));
            m_EntityHeader = new HttpEntityHeader();
            m_ResponseHeader = new HttpResponseHeader();
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

        public Byte[] Body
        {
            get { return m_Body; }
            set { m_Body = value; }
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

            byte[] msgheader = System.Text.Encoding.ASCII.GetBytes(strBldr.ToString());

            byte[] arry = new byte[msgheader.Length + m_Body.Length];
            msgheader.CopyTo(arry, 0);
            m_Body.CopyTo(arry, msgheader.Length);
            
            return arry;
        }
    }
}
