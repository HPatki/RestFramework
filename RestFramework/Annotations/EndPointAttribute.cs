using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using RestFramework.Interface;

namespace RestFramework.Annotations
{
    [AttributeUsage(AttributeTargets.Method)]
    sealed class EndPointAttribute : Attribute, Param
    {
        String m_Route;
        String m_MethodType;
        String m_Produces;
        String m_Consumes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="route">Route</param>
        /// <param name="Method">Method</param>
        /// <param name="produces">Produces</param>
        public EndPointAttribute(String route, String Method = "GET", String consumes="application/octet", String produces = "application/octet")
        {
            m_Route = route;
            m_MethodType = Method.ToUpper();
            m_Produces = produces;
            m_Consumes = consumes;
        }

        internal String Route
        {
            get { return m_Route; }
        }

        internal String Method
        {
            get { return m_MethodType; }
        }

        internal String Produces
        {
            get { return m_Produces; }
        }
    }
}
