﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestFramework.Exceptions
{
    class URLNotFound : Exception
    {
        internal URLNotFound(String URL)
            : base(URL + " not found")
        {

        }
    }
}
