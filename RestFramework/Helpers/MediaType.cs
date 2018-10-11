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
    }
}
