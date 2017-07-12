using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using RestFramework.Annotations;

namespace RestFramework.Broker
{
    sealed class ComponentMethodMapper
    {
        private Object mSingleTonInstance;
        private Dictionary<String, MethodDetails> mMethods;

        public ComponentMethodMapper(Type T)
        {
            mSingleTonInstance = Activator.CreateInstance(T);
            mMethods = new Dictionary<string, MethodDetails>();
        }

        public void AddMethodDetails(ControllerMethodAttribute MethodAttrib, MemberInfo info)
        {
            ParameterInfo[] paramInfo = ((MethodInfo)info).GetParameters();

            MethodDetails newOne = new MethodDetails();
            newOne.MethodName =  info.Name;
            mMethods.Add(MethodAttrib.Route, newOne);

            foreach (ParameterInfo Param in paramInfo)
            {
                object[] bodyParams = Param.GetCustomAttributes(typeof(MethodBodyParam), false);
                object[] QueryParams = Param.GetCustomAttributes(typeof(MethodQueryParam), false);
                object[] RequestParams = Param.GetCustomAttributes(typeof(MethodRequestParam), false);
            }
        }
    }
}
