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
            Byte[] retVal = null;

            switch (produces)
            {
                case MediaType.APPLICATION_JSON:
                    //the input object can be a 'Object' or a more basic type.
                    //in more basic types it could be a string that is already JSON formed
                    //or could be a plain string, boolean, char or numeric type
                    {
                        String objString = val as System.String;
                        String json = "{\"name\":" + "\"" + objString + "\"}";
                        retVal = System.Text.Encoding.UTF8.GetBytes(json);
                    }
                    break;
                case MediaType.TEXT_PLAIN:
                    {
                        String objString = val as System.String;
                        retVal = System.Text.Encoding.UTF8.GetBytes(objString);
                    }
                    break;
            }
           
            return retVal;
        }
    }
}
