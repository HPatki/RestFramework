using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.IO;

using RestFramework.Helpers;

namespace RestFramework.Transport
{
    class HttpRequest
    {
        private class HttpHeader
        {
            private static Regex    m_parser = new Regex("\r\n");
            private Dictionary<String, String>  m_Headers = new Dictionary<string,string>();
            private StringBuilder               m_RawContents = new StringBuilder();
            private String                      m_RawContentsStr = null;
            private Method                      m_method;
            private String                      m_RequestURI;
            private String                      m_HttpVersion;

            public HttpHeader ConcatenateRawContent (String content)
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
                m_Headers.TryGetValue("CONTENT-LENGTH", out length);
                if (null != length)
                    return Convert.ToInt32(length);
                return 0;
            }

            public void ExtractHeaders()
            {
                var headers = m_parser.Split(GetRawContents());
                for (int i = 0; i < headers.Length; ++i )
                {
                    String[] parts = headers[i].Split(':');
                    if (parts.Length > 1)
                    {
                        m_Headers.Add(parts[0].ToUpper(), parts[1]);
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

            public String getRequestURI ()
            {
                return m_RequestURI;
            }

            public String getHTTPVersion()
            {
                return m_HttpVersion;
            }

        }

        private class HttpBody
        {
            private MemoryStream bc = new MemoryStream();

            public void addBodyContent(ref byte[] content)
            {
                bc.Seek(0, SeekOrigin.End);
                bc.Write(content,0,content.Length);
            }

            public override String ToString()
            {
                String ret = null;
                StreamWriter wrtr = new StreamWriter(new FileStream("output.txt",FileMode.Create),Encoding.UTF8);
                
                wrtr.Write(Encoding.UTF8.GetString(bc.ToArray()));
                wrtr.Close();
                return ret;
            }
        }

        private HttpHeader  m_Header = new HttpHeader();
        private HttpBody    m_Body = new HttpBody();

        public HttpRequest ConcatenateRawHeaderContent(String content)
        {
            m_Header.ConcatenateRawContent(content);
            return this;
        }

        public int GetLengthOfBody()
        {
            return m_Header.GetLengthOfBody();
        }

        public HttpRequest ExtractHeaders()
        {
            m_Header.ExtractHeaders();
            return this;
        }

        public void ConcatenateBodyContent(byte[] content)
        {
            m_Body.addBodyContent(ref content);
        }

        public String GetBody()
        {
            return m_Body.ToString();
        }

        public Method getMethod()
        {
            return m_Header.getMethod();
        }

        public String getRequestURI()
        {
            return m_Header.getRequestURI();
        }

        public String getHTTPVersion()
        {
            return m_Header.getHTTPVersion();
        }
    }
}
