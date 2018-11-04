using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace HttpdServer.Helpers
{
    public enum MediaType
    {
        APPLICATION_JSON, 
        APPLICATION_OCTET_STREAM,
        TEXT_PLAIN,
        TEXT_HTML,
        APPLICATION_JPG,
        APPLICATION_PNG
    }

    public class MediaTypeContent
    {
        public static String GetContentType(MediaType typ)
        {
            String ct = null;

            switch (typ)
            {
                case MediaType.APPLICATION_JSON:
                    ct = "content-type:application/json\r\n";
                    break;
                case MediaType.APPLICATION_OCTET_STREAM:
                    ct = "content-type:application/octet\r\n";
                    break;
                case MediaType.TEXT_HTML:
                    ct = "content-type:text/html\r\n";
                    break;
                case MediaType.TEXT_PLAIN:
                    ct = "content-type:text/plain;charset=utf-8\r\n";
                    break;
                case MediaType.APPLICATION_JPG:
                    ct = "content-type:image/jpeg\r\n";
                    break;
                case MediaType.APPLICATION_PNG:
                    ct = "content-type:image/png\r\n";
                    break;
            }

            return ct;
        }

    }
}
