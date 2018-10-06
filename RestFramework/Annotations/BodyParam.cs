using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestFramework.Interface;

namespace RestFramework.Annotations
{
    [AttributeUsage (AttributeTargets.Parameter) ]
    sealed public class BodyParam : BaseAttribute
    {
        public BodyParam(String name) : base (name)
        {
            
        }
    }
}
