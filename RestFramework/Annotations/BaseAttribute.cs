using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpdServer.Transport;
using HttpdServer.Helpers;

namespace RestFramework.Annotations
{
    public abstract class BaseAttribute : Attribute
    {
        String m_varname;
        Type m_type;

        public BaseAttribute(String name)
        {
            m_varname = name;
        }

        internal String getName()
        {
            return m_varname;
        }

        internal void setType(Type T)
        {
            m_type = T;
        }

        internal Type getType()
        {
            return m_type;
        }

        internal Object Convert(String parsedValue)
        {
            return parsedValue;
        }
    }

    public class HttpResponse : BaseAttribute
    {
        private HttpdServer.Transport.HttpResponse m_HttpResponse;

        public HttpResponse()
            : base("HttpResponse")
        {
            m_HttpResponse = new HttpdServer.Transport.HttpResponse();
        }

        public String ContentEncoding
        {
            get { return m_HttpResponse.ContentEncoding; }
            set { m_HttpResponse.ContentEncoding = value; }
        }

        public UInt64 ContentLength
        {
            get { return m_HttpResponse.ContentLength; }
            set { m_HttpResponse.ContentLength = value; }
        }

        public MediaType ContentType
        {
            get { return m_HttpResponse.ContentType; }
            set { m_HttpResponse.ContentType = value; }
        }

        public Int32 StatusCode
        {
            get { return m_HttpResponse.StatusCode; }
            set { m_HttpResponse.StatusCode = value; }
        }

        public String StatusDesc
        {
            get { return m_HttpResponse.StatusDesc; }
            set { m_HttpResponse.StatusDesc = value; }
        }

        public void AddResponseHeader(String name, Object val)
        {
            m_HttpResponse.AddResponseHeader(name, val);
        }

        public HttpBody Body
        {
            get { return m_HttpResponse.Body; }
        }

        public byte[] Bytes()
        {
            return m_HttpResponse.Bytes();
        }
    }

}
