using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestFramework.Exceptions
{
    class SameEndPointURL : Exception 
    {
        internal SameEndPointURL(String URL)
            : base(URL + " maps to multiple end-points. Endpoint URLs have to be unique")
        {

        }
    }
}
