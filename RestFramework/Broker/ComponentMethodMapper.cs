using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using System.Reflection;
using RestFramework.Annotations;
using RestFramework.Interface;

namespace RestFramework.Broker
{
    sealed internal class ComponentMethodMapper
    {
        private Delegate m_delegate;
        private MethodInfo m_methodInfo;
        private List<Param> m_SequenceOfParams = new List<Param>();

        public void AddMethodDetails(object T, MethodInfo info, String URI)
        {
            m_methodInfo = info;

            m_delegate = Delegate.CreateDelegate(Program.getDelegateFactory().CreateDelegateType(info),
                T, m_methodInfo);

            ParameterInfo[] paramInfo = m_methodInfo.GetParameters();

            foreach (ParameterInfo Param in paramInfo)
            {
                object[] bodyParams = Param.GetCustomAttributes(typeof(BodyQueryParam), false);
                if (0 != bodyParams.Length)
                {
                    var param = bodyParams[0] as Param;
                    m_SequenceOfParams.Add(param);
                    continue;
                }

                object[] PQueryParams = Param.GetCustomAttributes(typeof(PathQueryVariable), false);
                if (0 != PQueryParams.Length)
                {
                    PathQueryVariable variable = PQueryParams[0] as PathQueryVariable;
                    variable.setPosInURL(Param.Position);
                    variable.setType(Param.ParameterType);
                    var param = PQueryParams[0] as Param;

                    m_SequenceOfParams.Add(param);

                    continue;
                }

                object[] BQueryParams = Param.GetCustomAttributes(typeof(BodyParam), false);
                if (0 != BQueryParams.Length)
                {
                    var param = BQueryParams[0] as Param;
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
                    var param = RequestParams[0] as Param;
                    
                    m_SequenceOfParams.Add(param);

                    continue;
                }

            }
        }

        internal List<Param> getParamList()
        {
            return m_SequenceOfParams;
        }

        internal object DynamicInvoke(object[] paramList)
        {
            return m_delegate.DynamicInvoke(paramList);
        }
    }
}
