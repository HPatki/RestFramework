using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using RestFramework.Annotations;
using HttpdServer.Helpers;

namespace RestApplication.controllers
{
    [RouteAttribute ("/advanced")] 
    public class AdvancedController
    {
        //BodyParam 
        [EndPointAttribute(route: "/file", consumes: MediaType.APPLICATION_OCTET_STREAM, 
            produces:MediaType.TEXT_PLAIN,method: "POST")]
        public String ReadFile([BodyParam("file")]MemoryStream InputFile, HttpResponse response)
        {
            BinaryWriter Wrtr = new BinaryWriter (new FileStream("F:/tmp/dummy.png",FileMode.Create));
            Wrtr.Write(InputFile.ToArray());
            Wrtr.Flush();
            Wrtr.Close();
            response.StatusCode = 200;
            response.AddResponseHeader("Content-Disposition", "attachment;filename=" + "dummy.png");
            return "Resource Created successfully";
        }

        //BodyParam 
        [EndPointAttribute(route: "/file",produces: MediaType.APPLICATION_PNG)]
        public MemoryStream ReadFile(HttpResponse response)
        {
            response.StatusCode = 200;
            Stream strm = new FileStream("F:/tmp/dummy.png", FileMode.Open);
            BinaryReader Rdr = new BinaryReader(strm);
            List<Byte> returned = new List<Byte>();
            while (true)
            {
                try
                {
                    Byte read = Rdr.ReadByte();
                    returned.Add(read);
                }
                catch(System.IO.EndOfStreamException)
                {
                    Rdr.Close();
                    break;
                }
            }
            response.AddResponseHeader("Content-Disposition", "attachment;filename=" + "dummy.png");
            return new MemoryStream(returned.ToArray());
        }
    }
}
