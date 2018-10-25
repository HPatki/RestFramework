using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpdServer.Exceptions
{
    class MethodNotImplemented : Exception
    {
        public MethodNotImplemented(String message)
            : base(message)
        {

        }
    }
}
