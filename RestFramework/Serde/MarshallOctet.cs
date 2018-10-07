using System;
using System.Collections.Generic;
using System.Text;

using RestFramework.Transport;
using RestFramework.Helpers;

namespace RestFramework.Serde
{
    class MarshallOctet
    {
        internal Byte[] marshall(HttpResponse response, Byte[] body)
        {
            if (response.StatusCode == 0)
            {
                //which indicates that the user did not set this
                response.StatusCode = 200;
            }

            response.StatusDesc = StatusCodeDesc.GetStatusDesc(response.StatusCode);
            response.ContentType = "application/octet";
            response.ContentLength = Convert.ToUInt64(body.Length);

            return null;
        }
    }
}
