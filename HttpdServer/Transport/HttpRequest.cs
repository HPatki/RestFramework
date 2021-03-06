﻿using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.IO;

using HttpdServer.Helpers;

namespace HttpdServer.Transport
{
    public class HttpRequest
    {
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

        public void SetLengthOfBody(Int64 len)
        {
            m_Body.SetLengthOfBody(len);
        }

        public HttpRequest ExtractHeaders()
        {
            m_Header.ExtractHeaders();
            return this;
        }

        /*public void ConcatenateBodyContent(byte[] content, Int32 BytesToSkip, Int64 len)
        {
            m_Body.addBodyContent(content,BytesToSkip,len);
        }*/

        public HttpBody GetBody()
        {
            return m_Body;
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

        public String GetHeaderValue(String header)
        {
            return m_Header.GetHeaderValue(header);
        }
    }
}
