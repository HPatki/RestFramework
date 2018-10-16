using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RestFramework.Transport
{
    internal class HttpBody
    {
        private Byte[] bc;
        private Int64 m_PresentLen;

        internal void SetLengthOfBody(Int64 len)
        {
            bc = new Byte[len];
            m_PresentLen = 0;
        }

        internal void addBodyContent(byte[] content, Int64 len)
        {
            Int64 upto = m_PresentLen + len;
            for (Int64 i = m_PresentLen, j = 0; i < upto; ++i, ++j)
            {
                try
                {
                    bc[i] = content[j];
                    ++m_PresentLen;
                }
                catch (Exception err)
                {
                    System.Console.WriteLine("Error");
                }
            }
        }

        internal Byte[] GetBody
        {
            get { return bc; }
        }
    }
}
