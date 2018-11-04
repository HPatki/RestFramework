using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

using HttpResp = RestFramework.Annotations.HttpResponse;
using HttpdServer.Transport;
using HttpdServer.Helpers;

using JSON = System.Runtime.Serialization.Json.DataContractJsonSerializer;

namespace RestFramework.Serde
{
    class MarshallOctet
    {
        unsafe internal static Byte[] marshall(HttpResp response, Object val,
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
                                    produces : response.ContentType;
            
            //package the body correctly
            Byte[] BodyContent = GetByte(val, produces);
            response.ContentLength = (UInt64)BodyContent.Length;
            
            HttpBody Body = response.Body;
            Body.SetLengthOfBody(BodyContent.Length);
            Body.SetBodyContent(BodyContent);            
            return response.Bytes();
        }

        public static Byte[] GetByte(Object val, MediaType produces)
        {
            Type TypeOfObject = val.GetType();
            Type objectType = TypeOfObject.BaseType;
            Byte[] retVal = null;

            switch (produces)
            {
                case MediaType.APPLICATION_JSON:
                    {
                        if ((objectType == typeof(System.ValueType)) ||
                             (val.GetType().FullName.Equals("System.String")))
                        {
                            String objString = val as System.String;
                            String json = "{\"name\":" + "\"" + objString + "\"}";
                            retVal = System.Text.Encoding.UTF8.GetBytes(json);
                        }
                        else
                        {
                            //object to be serialised
                            MemoryStream strm = new MemoryStream();
                            JSON serialised = new JSON(TypeOfObject);
                            serialised.WriteObject(strm,val);
                            return strm.ToArray();
                        }
                    }
                    break;
                default:
                    {
                        if ((objectType == typeof(System.ValueType)))
                        {
                            if (val.GetType() == typeof(int) ||
                                val.GetType() == typeof(long) ||
                                val.GetType() == typeof(double) ||
                                val.GetType() == typeof(short) ||
                                val.GetType() == typeof(bool) ||
                                val.GetType() == typeof(char))
                            {
                                //has to be sent as string
                                String objString = val.ToString();
                                retVal = System.Text.Encoding.UTF8.GetBytes(objString);
                            }
                        }
                        else //an object
                        {
                            if (val.GetType() == typeof(System.String))
                            {
                                String objString = val.ToString();
                                retVal = System.Text.Encoding.UTF8.GetBytes(objString);
                            }
                            //check if the object has ToArray method returning a byte[]
                            //if not throw error
                            else
                            {
                                MethodInfo MInfo = val.GetType().GetMethod("ToArray");
                                if (MInfo != null)
                                {
                                    ParameterInfo PInfo = MInfo.ReturnParameter;
                                    if (PInfo.ParameterType == typeof(System.Byte[]))
                                    {
                                        retVal = (System.Byte[])MInfo.Invoke(val, null);

                                    }
                                }
                                else
                                {
                                    //throw error
                                }
                            }

                        }
                    }
                    break;
            }

            return retVal;
        }

        
    }
}
