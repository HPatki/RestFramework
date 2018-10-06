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

        internal static object[] Extract(HttpRequest request, List<BaseAttribute> parameters, HttpResponse response)
        {
            String uri = request.getRequestURI();
            List<Object> objects = new List<object>();
            Object ParamToAdd = null;

            foreach (BaseAttribute p in parameters)
            {
                if (p is HttpResponse)
                {
                    objects.Add(response);
                }

                if (p is PathVariable)
                {
                    Int32 pos = ((PathVariable)p).getPosInURL();
                    String[] spit = uri.Substring(pos).Split('/');
                    ParamToAdd = spit[0];
                    
                }

                if (p is PathQueryVariable)
                {
                    String V_Name =((PathQueryVariable)p).getName();
                    Regex parser = new Regex("(" + V_Name + "=.*)");
                    MatchCollection splits = parser.Matches(uri);
                    if (splits.Count == 1)
                    {
                        String[] split = splits[0].Value.Split(ExtractMethodParams.m_PathQueryVarSplitter);
                        ParamToAdd = split[1];
                    }

                }

                if (p is HeaderParam)
                {
                    String headerval = request.GetHeaderValue(p.getName());
                    ParamToAdd = headerval;
                }

                objects.Add(Convert.ChangeType(ParamToAdd, p.getType()));
            }

            return objects.ToArray();
        }

    }
}
