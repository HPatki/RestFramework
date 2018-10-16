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
using RestFramework.Exceptions;

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
            Int32 len;
            foreach (BaseAttribute p in parameters)
            {
                if (p is HttpResponse)
                {
                    objects.Add (response);//special handling !!
                    continue;
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
                    ParamToAdd = request.GetBody();
                }

                if (p is BodyQueryParam)
                {
                    //TODO
                }

                objects.Add(ChangeType(ParamToAdd, p.getType(), consumes));
            }

            return objects.ToArray();
        }

        private static Object ChangeType (Object ParamToAdd, Type T, MediaType consumes)
        {
            Object toReturn = null;
            Type ParamType = ParamToAdd.GetType(); //string or Byte[]

            //it could be JSON
            if (MediaType.APPLICATION_JSON == consumes)
            {
                JSON Deser = new JSON(T);
                MemoryStream stream = null;

                if (ParamType == typeof(System.Byte[]))
                {
                    stream = new MemoryStream((Byte[])ParamToAdd);
                }
                else //it will always be string
                {
                    stream = new MemoryStream(
                        System.Text.Encoding.UTF8.GetBytes((String)ParamToAdd));
                }

                try
                {
                    toReturn = Deser.ReadObject(stream);
                }
                catch (Exception err)
                {
                    throw new UnsupportedMediaType(err.Message);
                }
            }
            else 
            {
                //based on T, convert
                if ( (T == typeof (int)) || (T == typeof (long)) || (T == typeof(double))
                    || (T == typeof(short)) || (T == typeof (char)) || (T == typeof(bool)) ) 
                {
                    Double a;
                    Double.TryParse((String)ParamToAdd, out a);
                    toReturn = Convert.ChangeType(a, T);
                }
                else if (T == typeof(System.String))
                {
                    toReturn = Convert.ChangeType(ParamToAdd, T);
                }
                else
                {
                    //check if object has a constructor taking byte
                }

                if (ParamToAdd.GetType().BaseType == typeof(System.ValueType))
                {
                    //??
                }
                else
                {
                    ConstructorInfo[] infor = T.GetConstructors();
                    foreach (ConstructorInfo info in infor)
                    {
                        var ParamInfo = info.GetParameters();
                        if (1 == ParamInfo.Length && ParamInfo[0].ParameterType == typeof(Byte[]))
                        {
                            Object[] arguments = new Object[1];
                            arguments[0] = ParamToAdd;
                            toReturn = info.Invoke(arguments);
                            break;
                        }
                    }
                }
            }

            return toReturn;
        }
    }
}
