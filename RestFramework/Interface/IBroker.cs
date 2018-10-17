using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
using HttpdServer.Transport;

namespace RestFramework.Interface
{
    interface IBroker
    {
        Byte[] Process(HttpRequest request);
    }
}

