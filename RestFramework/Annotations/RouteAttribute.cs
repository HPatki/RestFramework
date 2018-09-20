using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestFramework.Interface;

namespace RestFramework.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    sealed public class RouteAttribute : Attribute, Param
    {
        String mRoute;

        public String Route 
        {
            get { return mRoute;}
        }

        public RouteAttribute(String route)
        {
            mRoute = route;
        }
    }
}
