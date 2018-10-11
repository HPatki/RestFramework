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

        internal void addBodyContent(ref byte[] content)
        {
            bc.Seek(0, SeekOrigin.End);
            bc.Write(content, 0, content.Length);
        }

        internal Byte[] GetBody
        {
            get { return bc.GetBuffer(); }
        }

        internal Int64 GetBodyLength
        {
            get { return bc.Length; }
        }
    }
}
