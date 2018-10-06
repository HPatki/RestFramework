using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestFramework.Annotations
{
    public abstract class BaseAttribute : Attribute
    {
        String m_varname;
        Type m_type;

        public BaseAttribute(String name)
        {
            m_varname = name;
        }

        internal String getName()
        {
            return m_varname;
        }

        internal void setType(Type T)
        {
            m_type = T;
        }

        internal Type getType()
        {
            return m_type;
        }

        internal Object Convert(String parsedValue)
        {
            return parsedValue;
        }
    }
}
