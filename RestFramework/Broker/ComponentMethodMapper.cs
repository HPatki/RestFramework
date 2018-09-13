using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using RestFramework.Annotations;
using RestFramework.Interface;

namespace RestFramework.Broker
{
    sealed class ComponentMethodMapper
    {
        public Delegate     m_delegate;
        private MethodInfo  m_methodInfo;
        List<Param>         m_SequenceOfParams = new List<Param>();
        
        public void AddMethodDetails(object T, MethodInfo info)
        {
            m_methodInfo = info;

            m_delegate = Delegate.CreateDelegate(Program.getDelegateFactory().CreateDelegateType(info), 
                T, m_methodInfo) ;

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
                    var param = PQueryParams[0] as Param;
                    m_SequenceOfParams.Add(param);
                    continue;
                }

                object[] BQueryParams = Param.GetCustomAttributes(typeof(BodyQueryParam), false);
                if (0 != BQueryParams.Length)
                {
                    var param = BQueryParams[0] as Param;
                    m_SequenceOfParams.Add(param);
                    continue;
                }

                object[] RequestParams = Param.GetCustomAttributes(typeof(PathVariable), false);
                {
                    if (0 != RequestParams.Length)
                    {
                        var param = RequestParams[0] as Param;
                        m_SequenceOfParams.Add(param);
                        continue;
                    }
                }
            }
        }
    }
}
