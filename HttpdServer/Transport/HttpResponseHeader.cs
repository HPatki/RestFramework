using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpdServer.Transport
{
    public class ResponseHeaderIterator : IEnumerator
    {
        Dictionary<String, Object>.Enumerator m_Itor;

        internal ResponseHeaderIterator(Dictionary<String, Object>.Enumerator itor)
        {
            m_Itor = itor;
        }

        public object Current
        {
            get { return m_Itor.Current; }
        }

        public Boolean MoveNext()
        {
            if (m_Itor.MoveNext())
                return true;

            return false;
        }

        public void Reset()
        {
            //nothing
        }
    }

    class HttpResponseHeader : IEnumerable
    {
        Dictionary<String, Object> m_headers;
        Boolean m_ItorConstructed = false;
        
        internal HttpResponseHeader()
        {
            m_headers = new Dictionary<string, object>();
        }

        internal void AddHeader(String name, Object val)
        {
            m_headers.Add(name, val);
        }

        public IEnumerator GetEnumerator()
        {
            return new ResponseHeaderIterator(m_headers.GetEnumerator());
        }
        
    }
}
