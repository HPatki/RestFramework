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

        public byte[] Bytes()
        {   
            /*BinaryReader rdr = new BinaryReader(File.Open(@"f:\D\Doodles\Amazon-TimeLine.png", FileMode.Open));
            List<byte> lByte = new List<byte>();
            int read = 10, readSoFar=0;
            byte[] arr;
            do
            {
                arr = rdr.ReadBytes(read);
                readSoFar += arr.Length;
                foreach (byte b in arr)
                    lByte.Add(b);
                
            } while (arr.Length >= read);

            var lByteArr = lByte.ToArray();
            rdr.Close();*/
            StringBuilder strBldr = new StringBuilder();
            strBldr.Append("HTTP/1.1 " + StatusCode + " Bad Gateway\r\n");
            //strBldr.Append("content-type: image/png\r\n");
            //strBldr.Append("content-type: text/plain\r\n");
            byte[] payload = System.Text.Encoding.UTF8.GetBytes("熊本大学　イタリア　宝島");
            strBldr.Append("content-length:" + payload.Length + "\r\n");
            strBldr.Append("\r\n");
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(strBldr.ToString());

            //byte[] arry = new byte[msg.Length + lByteArr.Length];
            byte[] arry = new byte[msg.Length + payload.Length];
            msg.CopyTo(arry, 0);
            //lByteArr.CopyTo(arry, msg.Length);
            //byte[] cc = System.Text.Encoding.UTF8.GetBytes("熊本大学　イタリア　宝島");
            payload.CopyTo(arry, msg.Length);
            
            return arry;
        }
    }
}
