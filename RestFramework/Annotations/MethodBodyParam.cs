using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestFramework.Annotations
{
    [AttributeUsage (AttributeTargets.Parameter)]
    class MethodBodyParam : Attribute
    {
        String mMethodArgs;

        public MethodBodyParam(String Arg)
        {
            mMethodArgs = Arg;
        }
    }
}
