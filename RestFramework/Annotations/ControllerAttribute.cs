using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestFramework.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    sealed class ControllerAttribute : Attribute
    {
        String mRoute;

        public String Route 
        {
            get { return mRoute;}
        }

        public ControllerAttribute(String route)
        {
            mRoute = route;
        }
    }
}
