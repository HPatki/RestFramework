using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using RestFramework.Interface;
using MethodType = RestFramework.Helpers.Method;

namespace RestFramework.Annotations
{
    [AttributeUsage(AttributeTargets.Method)]
    sealed public class EndPointAttribute : Attribute, Param
    {
        String      m_Route;
        MethodType  m_MethodType;
        String      m_Produces;
        String      m_Consumes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="route">Route</param>
        /// <param name="Method">Method</param>
        /// <param name="produces">Produces</param>
        public EndPointAttribute(String route, String method = "GET", String consumes="application/octet", String produces = "application/octet")
        {
            m_Route = route;
            switch(method)
            {
                case "GET":
                    m_MethodType = MethodType.GET;
                    break;
            }
            
            m_Produces = produces;
            m_Consumes = consumes;
        }

        internal String Route
        {
            get { return m_Route; }
        }

        internal MethodType Method
        {
            get { return m_MethodType; }
        }

        internal String Produces
        {
            get { return m_Produces; }
        }
    }
}
