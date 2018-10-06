using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestFramework.Interface;

namespace RestFramework.Annotations
{
    sealed public class PathQueryVariable : BaseAttribute
    {
        Int32 m_posInURL;
        
        public PathQueryVariable(String name) : base (name)
        {
            
        }

        internal void setPosInURL(Int32 pos)
        {
            m_posInURL = pos;
        }

        internal Int32 getPosInURL()
        {
            return m_posInURL;
        }

    }
}
