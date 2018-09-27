using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using RestFramework.Transport;
using RestFramework.Interface;
using RestFramework.Annotations;

namespace RestFramework.Helpers
{
    internal class ExtractMethodParams
    {
        private static char[] m_PathQueryVarSplitter = { '=', '&' };

        internal static object[] Extract(HttpRequest request, List<Param> parameters)
        {
            String uri = request.getRequestURI();
            List<Object> objects = new List<object>();

            foreach (Param p in parameters)
            {
                if (p is PathVariable)
                {
                    Int32 pos = ((PathVariable)p).getPosInURL();
                    String[] spit = uri.Substring(pos).Split('/');
                    objects.Add(spit[0]);
                }

                if (p is PathQueryVariable)
                {
                    String V_Name =((PathQueryVariable)p).getName();
                    Regex parser = new Regex("(" + V_Name + "=.*)");
                    MatchCollection splits = parser.Matches(uri);
                    if (splits.Count == 1)
                    {
                        String[] split = splits[0].Value.Split(ExtractMethodParams.m_PathQueryVarSplitter);
                        objects.Add(split[1]);
                    }

                }

            }

            return objects.ToArray();
        }

    }
}
