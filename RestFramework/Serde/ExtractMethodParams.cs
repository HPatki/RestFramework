using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using JSON = System.Runtime.Serialization.Json.DataContractJsonSerializer;
using System.IO;
using System.Reflection;

using HttpdServer.Transport;
using RestFramework.Interface;
using RestFramework.Annotations;
using HttpdServer.Helpers;
using RestFramework.Exceptions;
using HttpResp = RestFramework.Annotations.HttpResponse;

namespace RestFramework.Serde
{
    internal class ExtractMethodParams
    {
        private static char[] m_PathQueryVarSplitter = { '=', '&' };

        internal static object[] Extract(HttpRequest request, BodyBinaryExtractor[] BodyArguments,
            HttpResp response,List<BaseAttribute> parameters, MediaType consumes
            )
        {
            String uri = request.getRequestURI();
            List<Object> objects = new List<object>();
            Object ParamToAdd = null;
            Int32 len;
            foreach (BaseAttribute p in parameters)
            {
                if (p is HttpResp)
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

                if (p is BodyParam || p is BodyQueryParam)
                {
                    //the body can be MultiPart Form Data. If so extract from the Body
                    //else tje body itself denotes the data

                    ParamToAdd = ExtractBodyParameter(request, BodyArguments, p); 
                    
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

                if (ParamType == typeof(BodyBinaryExtractor))
                {
                    var extractor = ParamToAdd as BodyBinaryExtractor;
                    stream = new MemoryStream(extractor.FileContent, (Int32)extractor.StartPos,(Int32)extractor.EndPos);
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
                toReturn = ExtractMethodParams.Convert(T, ParamToAdd); 
            }

            return toReturn;
        }

        private static Object ExtractBodyParameter(HttpRequest request, BodyBinaryExtractor[] BodyArguments, BaseAttribute p)
        {
            Object toReturn = null;
            //name based lookup
            foreach (BodyBinaryExtractor be in BodyArguments)
            {
                //internally all strings are being converted to UPPER case
                if (be.ParamName.Equals(p.getName().ToUpper(), StringComparison.CurrentCultureIgnoreCase))
                {
                    toReturn = be;
                    break;
                }
            }

            return toReturn;
        }

        private static Object Convert(Type T, Object ParamToConvert)
        {
            IConvertible convertible = ParamToConvert as IConvertible;
            
            Object toReturn = null;

            if (T == typeof(Boolean))
            {
                toReturn = convertible.ToInt32(null);
            }
            else  if (T == typeof(Byte))
            {
                toReturn = convertible.ToInt32(null);
            }
            else  if (T == typeof(Char))
            {
                toReturn = convertible.ToChar(null);
            }
            else  if (T == typeof(DateTime))
            {
                toReturn = convertible.ToDateTime(null);
            }
            else  if (T == typeof(Decimal))
            {
                toReturn = convertible.ToDecimal(null);
            }
            else  if (T == typeof(Double))
            {
                toReturn = convertible.ToDouble(null);
            }
            else  if (T == typeof(Int16))
            {
                toReturn = convertible.ToInt16(null);
            }
            else  if (T == typeof(Int32))
            {
              toReturn = convertible.ToInt32(null);
            }
            else  if (T == typeof(Int64))
            {
               toReturn = convertible.ToInt64(null);
            }
            else  if (T == typeof(SByte))
            {
                toReturn = convertible.ToSByte(null);
            }
            else  if (T == typeof(Single))
            {
              toReturn = convertible.ToSingle(null);
            }
            else  if (T == typeof(String))
            {
              toReturn = convertible.ToString(null);
            }
            else  if (T == typeof(UInt16))
            {
                toReturn = convertible.ToUInt16(null);
            }
            else  if (T == typeof(UInt32))
            {
                toReturn = convertible.ToUInt32(null);
            }
            else  if (T == typeof(UInt64))
            {
                toReturn = convertible.ToUInt64(null);
            }
            else if (T is Object)
            {
                toReturn = convertible.ToType(T, null);
            }
            return toReturn;

            /*if (ParamToAdd.GetType().BaseType == typeof(System.ValueType))
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
            }*/
        }
    }
}
