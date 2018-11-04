using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HttpdServer.Transport
{
    public class HttpBody
    {
        private Byte[] bc;
        private Int64 m_PresentLen;

        public Int64 CurrentPos
        {
            get { return m_PresentLen; }
        }

        public void SetLengthOfBody(Int64 len)
        {
            bc = new Byte[len];
            m_PresentLen = 0;
        }

        public void SetBodyContent(Byte[] content)
        {
            bc = content;
        }

        public Int32 addBodyContent(byte[] content, Int32 BytesToSkip, Int64 len)
        {
            try
            {
                Array.Copy(content, BytesToSkip, bc, m_PresentLen, len - BytesToSkip);
                m_PresentLen += (len - BytesToSkip);
            }
            catch (Exception err)
            {
                System.Console.WriteLine(err.Message);
            }

            /*Int32 BytesRead = 0;

            Int64 upto = m_PresentLen + len - BytesToSkip;
            for (Int64 i = m_PresentLen, j = BytesToSkip; i < upto; ++i, ++j)
            {
                try
                {
                    Array.Copy(content, BytesToSkip, bc, m_PresentLen, len-BytesToSkip);
                    m_PresentLen += (len - BytesToSkip);
                    //bc[i] = content[j];
                    //++m_PresentLen;
                    //++BytesRead;
                }
                catch (Exception err)
                {
                    System.Console.WriteLine("Error");
                }
            }*/

            return (Int32)(len - BytesToSkip);
        }

        public Byte[] Body
        {
            get { return bc; }
            set { bc = value; }
        }
    }
}
