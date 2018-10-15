using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestFramework.Helpers
{
    public enum MediaType
    {
        APPLICATION_JSON, 
        APPLICATION_OCTET_STREAM,
        TEXT_PLAIN,
        TEXT_HTML,
        APPLICATION_JPG
    }

    public class MediaTypeContent
    {
        public static String GetContentType(MediaType typ)
        {
            String ct = null;

            switch (typ)
            {
                case MediaType.APPLICATION_JSON:
                    ct = "application/json";
                    break;
                case MediaType.APPLICATION_OCTET_STREAM:
                    ct = "application/octet";
                    break;
                case MediaType.TEXT_HTML:
                    ct = "text/html";
                    break;
                case MediaType.TEXT_PLAIN:
                    ct = "text/plain;charset=utf-8";
                    break;
                case MediaType.APPLICATION_JPG:
                    ct = "image/jpg";
                    break;
            }

            return ct;
        }

        public static Byte[] GetByte(Object val, MediaType produces)
        {
            Type objectType = val.GetType().BaseType;
            Byte[] retVal = null;

             switch (produces)
             {          
                case MediaType.APPLICATION_JSON:
                        {
                            if ( (objectType == typeof(System.ValueType) ) ||
                                 (val.GetType().FullName.Equals("System.String")))
                            {
                                String objString = val as System.String;
                                String json = "{\"name\":" + "\"" + objString + "\"}";
                                retVal = System.Text.Encoding.UTF8.GetBytes(json);
                            }
                            else
                            {
                                //object to be serialised
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
                                
                            }
                        }
                        break;
                }
            
            return retVal;
        }
    }
}
