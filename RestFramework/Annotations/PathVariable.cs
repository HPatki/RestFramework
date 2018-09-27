using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestFramework.Interface;

namespace RestFramework.Annotations
{
    [AttributeUsage(AttributeTargets.Parameter)]
    sealed public class PathVariable : Attribute, Param
    {
        String  m_varname;
        Int32   m_posInURL;
        Type    m_type;

        public PathVariable(String varname)
        {
            m_varname = varname;
        }

        public String getName ()
        {
            return m_varname;
        }

        public void setPosInURL(Int32 pos)
        {
            m_posInURL = pos;
        }

        public Int32 getPosInURL()
        {
            return m_posInURL;
        }

        public void setType(Type T)
        {
            m_type = T;
        }

        public Type getType()
        {
            return m_type;
        }

        public Object Convert(String parsedValue)
        {
            return parsedValue;
        }
    }
}
