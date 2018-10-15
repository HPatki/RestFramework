using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestFramework.Exceptions
{
    class UnsupportedMediaType : Exception
    {
        public UnsupportedMediaType(String message)
            : base(message)
        {

        }
    }
}
