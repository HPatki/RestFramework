using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using System.Reflection;
using RestFramework.Annotations;
using RestFramework.Interface;
using RestFramework.Transport;
using RestFramework.Helpers;

namespace RestFramework.Broker
{
    sealed internal class ComponentMethodMapper
    {
        //private Delegate    m_delegate;
        private MethodInfo  m_methodInfo;
        private List<BaseAttribute> m_SequenceOfParams = new List<BaseAttribute>();
        private MediaType   m_Produces;
        private MediaType   m_Consumes;
        private Object      m_obj;

        internal MethodInfo GetMethodInfo()
        {
            return m_methodInfo;
        }

        internal Object GetObject()
        {
            return m_obj;
        }

        internal void AddMethodDetails(object T, MethodInfo info, String URI, EndPointAttribute endPtAttr)
        {
            m_obj = T;

            m_methodInfo = info;
            
            //m_delegate = Delegate.CreateDelegate(Program.getDelegateFactory().CreateDelegateType(info),
            //    T, info);

            ParameterInfo[] paramInfo = m_methodInfo.GetParameters();

            foreach (ParameterInfo Param in paramInfo)
            {
                object[] bodyParams = Param.GetCustomAttributes(typeof(BodyQueryParam), false);
                if (0 != bodyParams.Length)
                {
                    var param = bodyParams[0] as BaseAttribute;
                    m_SequenceOfParams.Add(param);
                    continue;
                }

                object[] PQueryParams = Param.GetCustomAttributes(typeof(PathQueryVariable), false);
                if (0 != PQueryParams.Length)
                {
                    PathQueryVariable variable = PQueryParams[0] as PathQueryVariable;
                    variable.setPosInURL(Param.Position);
                    variable.setType(Param.ParameterType);
                    var param = PQueryParams[0] as BaseAttribute;

                    m_SequenceOfParams.Add(param);

                    continue;
                }

                object[] BQueryParams = Param.GetCustomAttributes(typeof(BodyParam), false);
                if (0 != BQueryParams.Length)
                {
                    var param = BQueryParams[0] as BaseAttribute;
                    m_SequenceOfParams.Add(param);
                    continue;
                }

                object[] RequestParams = Param.GetCustomAttributes(typeof(PathVariable), false);
                if (0 != RequestParams.Length)
                {
                    PathVariable variable = RequestParams[0] as PathVariable;
                    Regex parser = new Regex("{"+variable.getName()+"}");
                    String[] splits = parser.Split(URI);
                    variable.setPosInURL(splits[0].Length);
                    variable.setType(Param.ParameterType);
                    var param = RequestParams[0] as BaseAttribute;
                    
                    m_SequenceOfParams.Add(param);

                    continue;
                }

                if (Param.ParameterType == typeof(HttpResponse))
                {
                    m_SequenceOfParams.Add(new HttpResponse());
                    continue;
                }

            }
        }

        internal List<BaseAttribute> getParamList()
        {
            return m_SequenceOfParams;
        }

        internal Type GetReturnType ()
        {
            return m_methodInfo.ReturnParameter.ParameterType;
        }

        //internal object DynamicInvoke(object[] paramList)
        //{
        //    return m_delegate.DynamicInvoke(paramList);
        //}
    }
}
