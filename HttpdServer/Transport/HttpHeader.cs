using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using HttpdServer.Helpers;
using HttpdServer.Exceptions;

namespace HttpdServer.Transport
{
    public class HttpHeaderItem : IConvertible 
    {
        String m_Content;

        public HttpHeaderItem(String content)
        {
            m_Content = content;
        }

        public override string ToString()
        {
            return m_Content;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            Boolean outval;
            Boolean.TryParse(m_Content, out outval);
            return outval;
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.String;
        }

        public byte ToByte(IFormatProvider provider)
        {
            Byte outval;
            Byte.TryParse(m_Content,out outval);
            return outval;
        }

        public char ToChar(IFormatProvider provider)
        {
            Char outval;
            Char.TryParse(m_Content, out outval);
            return outval;
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new MethodNotImplemented("Method ToDateTime is not implemented");
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new MethodNotImplemented("Method ToDecimal is not implemented");
        }

        public double ToDouble(IFormatProvider provider)
        {
            return Double.Parse(m_Content);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return Int16.Parse(m_Content);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return Int32.Parse(m_Content);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return Int64.Parse(m_Content);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            sbyte outval;
            SByte.TryParse(m_Content, out outval);
            return outval;
        }

        public float ToSingle(IFormatProvider provider)
        {
            return float.Parse(m_Content);
        }

        public string ToString(IFormatProvider provider)
        {
            return m_Content;
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new MethodNotImplemented("Method ToType is not implemented");
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            UInt16 outval;
            UInt16.TryParse(m_Content, out outval);
            return outval;
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            UInt32 outval;
            UInt32.TryParse(m_Content, out outval);
            return outval;
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            UInt64 outval;
            UInt64.TryParse(m_Content, out outval);
            return outval;
        }
    }

    public class HttpHeader
    {
        private static Regex m_parser = new Regex("\r\n");
        private Dictionary<String, HttpHeaderItem> m_Headers = new Dictionary<string, HttpHeaderItem>();
        private StringBuilder m_RawContents = new StringBuilder();
        private String m_RawContentsStr = null;
        private Method m_method;
        private String m_RequestURI;
        private char[] m_HeaderSep = { ':' };
        
        private String m_HttpVersion;
         
        public HttpHeader ConcatenateRawContent(String content)
        {
            m_RawContents.Append(content);
            return this;
        }

        public String GetRawContents()
        {
            if (null == m_RawContentsStr)
                m_RawContentsStr = m_RawContents.ToString();

            return m_RawContentsStr;
        }

        public int GetLengthOfBody()
        {
            String length = null;
            HttpHeaderItem item;
            m_Headers.TryGetValue("CONTENT-LENGTH", out item);
            if (null != item)
                return Convert.ToInt32(item.ToString());
            return 0;
        }

        public void ExtractHeaders()
        {
            var headers = m_parser.Split(GetRawContents());
            for (int i = 0; i < headers.Length; ++i)
            {
                String[] parts = headers[i].Split(m_HeaderSep, 2);
                if (parts.Length > 1)
                {   
                    m_Headers.Add(parts[0].ToUpper(), new HttpHeaderItem(parts[1]));
                }
                else
                {
                    //can be the request-line. 
                    if (parts[0].Contains("HTTP"))
                    {
                        String[] requestLineSplit = parts[0].Split(' ');
                        switch (requestLineSplit[0])
                        {
                            case "POST":
                                m_method = Method.POST;
                                break;
                            case "GET":
                                m_method = Method.GET;
                                break;
                            case "PUT":
                                m_method = Method.PUT;
                                break;
                            case "DELETE":
                                m_method = Method.DELETE;
                                break;
                            default:
                                m_method = Method.ERROR;
                                break;
                        }
                        m_RequestURI = requestLineSplit[1];
                        m_HttpVersion = requestLineSplit[2];
                    }
                }
            }
        }

        public Method getMethod()
        {
            return m_method;
        }

        public String getRequestURI()
        {
            return m_RequestURI;
        }

        public String getHTTPVersion()
        {
            return m_HttpVersion;
        }

        public String GetHeaderValue(String header)
        {
            String val = null;
            HttpHeaderItem item;
            m_Headers.TryGetValue(header.ToUpper(), out item);
            if (null != item)
                val = item.ToString();
            return val;
        }

    }
}
