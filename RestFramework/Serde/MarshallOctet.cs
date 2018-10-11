﻿using System;
using System.Collections.Generic;
using System.Text;

using RestFramework.Transport;
using RestFramework.Helpers;

namespace RestFramework.Serde
{
    class MarshallOctet
    {
        internal static Byte[] marshall(HttpResponse response, Object val, Boolean UsrCodeHandledResponse,
            MediaType produces)
        {
            if (response.StatusCode == 0)
            {
                //which indicates that the user did not set this
                response.StatusCode = 200;
            }

            //return values can be Object, any built in Type, a file or nothing.
            //Process depending on return format specified by the user
 
            response.StatusDesc = StatusCodeDesc.GetStatusDesc(response.StatusCode);
            response.ContentType = response.ContentType.Equals("") == true ?
                                    MediaTypeContent.GetContentType(produces) : response.ContentType;
              
            String objString = val as System.String ;
            String json = "{\"name\":" + "\"" + objString + "\"}";
            response.Body = System.Text.Encoding.UTF8.GetBytes(json);
            response.ContentLength = (UInt64)response.Body.Length;

            return response.Bytes();
        }
    }
}
