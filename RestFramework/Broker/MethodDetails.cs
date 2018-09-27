using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using RestFramework.Annotations;

namespace RestFramework.Broker
{
    sealed class MethodDetails
    {
        String                      mMethodName;
        MethodInfo                  m_MethodInfo;
        List<BodyQueryParam>        mBodyParamDetails = new List<BodyQueryParam>();
        List<PathQueryVariable>     mQueryParamDetails = new List<PathQueryVariable>();
        List<PathVariable>          mRequestParamDetails = new List<PathVariable>();

        public String MethodName 
        {
            set { mMethodName = value; }
            get {return mMethodName; }
        }

        public MethodInfo MethodInfo
        {
            set {m_MethodInfo=value;}
            get { return m_MethodInfo; }
        }

        public void AddBodyParam(BodyQueryParam param)
        {
            mBodyParamDetails.Add(param);
        }

        public void AddQueryParam(PathQueryVariable param)
        {
            mQueryParamDetails.Add(param);
        }

        public void AddRequestParam(PathVariable param)
        {
            mRequestParamDetails.Add(param);
        }
    }
}
