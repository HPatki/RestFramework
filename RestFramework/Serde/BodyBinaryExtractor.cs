using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestFramework.Serde
{
    public class BodyBinaryExtractor
    {
        internal String m_ParamName;
        internal Byte[] m_FileContent;

        public String ParamName
        {
            get { return m_ParamName; }
            set { m_ParamName = value; }
        }

        public Byte[] FileContent
        {
            get { return m_FileContent; }
            set { m_FileContent = value; }
        }
    }
}
