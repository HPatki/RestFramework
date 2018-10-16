using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HttpResp = RestFramework.Annotations.HttpResponse;
using System.Reflection;
using RestFramework.Annotations;
using RestFramework.Interface;
using HttpdServer.Transport;
using HttpdServer.Helpers;

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

        internal MediaType Produces
        {
            get { return m_Produces; }
        }

        internal MediaType Consumes
        {
            get { return m_Consumes; }
        }

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

            m_Consumes = endPtAttr.Consumes;
            m_Produces = endPtAttr.Produces;
            
            //m_delegate = Delegate.CreateDelegate(Program.getDelegateFactory().CreateDelegateType(info),
            //    T, info);

            ParameterInfo[] paramInfo = m_methodInfo.GetParameters();

            foreach (ParameterInfo Param in paramInfo)
            {
                object[] baseAttr = Param.GetCustomAttributes(typeof(BaseAttribute), true);
                BaseAttribute attrib = null;
                
                if (0 != baseAttr.Length)
                {
                    attrib = baseAttr[0] as BaseAttribute;
                    attrib.setType(Param.ParameterType);
                }
                else
                {
                    if (Param.ParameterType == typeof(HttpResp))
                    {
                        m_SequenceOfParams.Add(new HttpResp());
                    }

                    continue;
                }


                if (attrib.GetType() == typeof(BodyQueryParam))
                {
                    m_SequenceOfParams.Add(attrib);
                    continue;
                }

                if (attrib.GetType() == typeof(PathQueryVariable))
                {
                    var variable = attrib as PathQueryVariable;
                    variable.setPosInURL(Param.Position);
                    m_SequenceOfParams.Add(variable);
                    continue;
                }

                if (attrib.GetType() == typeof(BodyParam))
                {
                    m_SequenceOfParams.Add(attrib);
                    continue;
                }

                if (attrib.GetType() == typeof(PathVariable))
                {
                    var variable = attrib as PathVariable;
                    Regex parser = new Regex("{"+variable.getName()+"}");
                    String[] splits = parser.Split(URI);
                    variable.setPosInURL(splits[0].Length);
                    
                    m_SequenceOfParams.Add(variable);

                    continue;
                }

                if (attrib.GetType() == typeof(HeaderParam))
                {
                    var param = attrib as HeaderParam;
                    m_SequenceOfParams.Add(param);
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
