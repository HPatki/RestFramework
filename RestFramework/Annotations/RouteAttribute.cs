using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestFramework.Interface;

namespace RestFramework.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    sealed public class RouteAttribute : BaseAttribute
    {

        public RouteAttribute(String route) : base (route)
        {
            
        }
    }
}
