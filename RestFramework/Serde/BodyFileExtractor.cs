using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace RestFramework.Serde
{
    public class BodyFileExtractor : BodyBinaryExtractor
    {
        private String  m_FileName;
        private String  m_FileType;
        
        public String FileName
        {
            get { return m_FileName; }
            set { m_FileName = value; }
        }

        public String FileType
        {
            get { return m_FileType; }
            set { m_FileType = value; }
        }

    }
}
