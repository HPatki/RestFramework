using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RestFramework.Transport
{
    internal class HttpBody
    {
        private MemoryStream bc = new MemoryStream();

        public void addBodyContent(ref byte[] content)
        {
            bc.Seek(0, SeekOrigin.End);
            bc.Write(content, 0, content.Length);
        }

        public override String ToString()
        {
            String ret = null;
            StreamWriter wrtr = new StreamWriter(new FileStream("output.txt", FileMode.Create), Encoding.UTF8);

            wrtr.Write(Encoding.UTF8.GetString(bc.ToArray()));
            wrtr.Close();
            return ret;
        }
    }
}
