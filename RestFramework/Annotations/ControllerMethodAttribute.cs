using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestFramework.Annotations
{
    [AttributeUsage(AttributeTargets.Method)]
    sealed class ControllerMethodAttribute : Attribute
    {
        String mRoute;
        String mMethodType;
        String mProduces;

        public ControllerMethodAttribute(String route, String Method = "GET", String produces = "application/octet")
        {
            mRoute = route;
            mMethodType = Method;
            mProduces = produces;
        }

        public String Route
        {
            get { return mRoute; }
        }

        public String Method
        {
            get { return mMethodType; }
        }

        public String Produces
        {
            get { return mProduces; }
        }
    }
}
