using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestFramework.Interface;

namespace RestFramework.Annotations
{
    [AttributeUsage(AttributeTargets.Parameter)]
    sealed public class PathVariable : BaseAttribute
    {
        String  m_varname;
        Int32   m_posInURL;
        Type    m_type;

        public PathVariable(String varname) : base (varname)
        {
            
        }

        public void setPosInURL(Int32 pos)
        {
            m_posInURL = pos;
        }

        public Int32 getPosInURL()
        {
            return m_posInURL;
        }

    }
}
