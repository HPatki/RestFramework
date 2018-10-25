using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestFramework.Exceptions
{
    class MethodNotImplemented : Exception
    {
        public MethodNotImplemented(String message)
            : base(message)
        {

        }
    }
}
