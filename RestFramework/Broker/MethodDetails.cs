using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestFramework.Annotations;

namespace RestFramework.Broker
{
    class MethodDetails
    {
        String                      mMethodName;
        List<MethodBodyParam>       mBodyParamDetails;
        List<MethodQueryParam>      mQueryParamDetails;
        List<MethodRequestParam>    mRequestParamDetails;

        public String MethodName 
        {
            set { mMethodName = value; }
            get {return mMethodName; }
        }

        public void AddBodyParam(MethodBodyParam param)
        {
            mBodyParamDetails.Add(param);
        }

        public void AddQueryParam(MethodQueryParam param)
        {
            mQueryParamDetails.Add(param);
        }

        public void AddRequestParam(MethodRequestParam param)
        {
            mRequestParamDetails.Add(param);
        }
    }
}
