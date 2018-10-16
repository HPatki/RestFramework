﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using HttpdServer.Helpers;

namespace HttpdServer.Transport
{
    public class HttpHeader
    {
        private static Regex m_parser = new Regex("\r\n");
        private Dictionary<String, String> m_Headers = new Dictionary<string, string>();
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
            m_Headers.TryGetValue("CONTENT-LENGTH", out length);
            if (null != length)
                return Convert.ToInt32(length);
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
            String val;
            m_Headers.TryGetValue(header.ToUpper(), out val);
            return val;
        }

    }
}
