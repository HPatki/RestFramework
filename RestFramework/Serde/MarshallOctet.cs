using System;
using System.Collections.Generic;
using System.Text;
using HttpResp = RestFramework.Annotations.HttpResponse;
using HttpdServer.Transport;
using HttpdServer.Helpers;

namespace RestFramework.Serde
{
    class MarshallOctet
    {
        internal static Byte[] marshall(HttpResp response, Object val,
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
            
            //package the body correctly
            String objString = val as System.String ;

            Byte[] BodyContent = MediaTypeContent.GetByte(val, produces);
            response.ContentLength = (UInt64)BodyContent.Length;
            
            HttpBody Body = response.Body;
            Body.SetLengthOfBody(BodyContent.Length);
            Body.addBodyContent(BodyContent, 0, BodyContent.Length);
            
            return response.Bytes();
        }
    }
}
