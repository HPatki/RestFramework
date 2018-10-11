using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using JSON = System.Runtime.Serialization.Json.DataContractJsonSerializer;
using System.IO;
using System.Reflection;

using RestFramework.Transport;
using RestFramework.Interface;
using RestFramework.Annotations;
using RestFramework.Helpers;

namespace RestFramework.Serde
{
    internal class ExtractMethodParams
    {
        private static char[] m_PathQueryVarSplitter = { '=', '&' };

        internal static object[] Extract(HttpRequest request, HttpResponse response,
            List<BaseAttribute> parameters, MediaType consumes
            )
        {
            String uri = request.getRequestURI();
            List<Object> objects = new List<object>();
            Object ParamToAdd = null;

            foreach (BaseAttribute p in parameters)
            {
                if (p is HttpResponse)
                {
                    ParamToAdd = response;
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
                    
                    //it could be JSON
                    if (MediaType.APPLICATION_JSON == consumes)
                    {
                        JSON Deser = new JSON(p.getType());
                        MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(headerval));
                        ParamToAdd = Deser.ReadObject(stream);
                    }
                    else
                    {
                        ParamToAdd = headerval;
                    }
                }

                if (p is BodyParam)
                {
                    Byte[] body = request.GetBody();
                    Int32 len = (Int32)request.GetBodyLength();
                    
                    //it could be JSON
                    if (MediaType.APPLICATION_JSON == consumes)
                    {
                        JSON Deser = new JSON(p.getType());
                        MemoryStream stream = new MemoryStream(body,0,len);
                        ParamToAdd = Deser.ReadObject(stream);
                    }
                    else
                    {
                        Type T = p.getType();
                        ConstructorInfo[] infor = T.GetConstructors();
                        foreach (ConstructorInfo info in infor)
                        {
                            var ParamInfo = info.GetParameters();
                            if (1 == ParamInfo.Length && ParamInfo[0].ParameterType == typeof(Byte[]))
                            {
                                Object[] arguments = new Object[1];
                                arguments[0] = body;
                                ParamToAdd = info.Invoke(arguments);
                            }
                        }
                    }
                }

                if (p is BodyQueryParam)
                {
                    //TODO
                }

                objects.Add(Convert.ChangeType(ParamToAdd, p.getType()));
            }

            return objects.ToArray();
        }

    }
}
