using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using RestFramework.Interface;
using MethodType = HttpdServer.Helpers.Method;
using HttpdServer.Helpers;

namespace RestFramework.Annotations
{
    [AttributeUsage(AttributeTargets.Method)]
    sealed public class EndPointAttribute : BaseAttribute
    {
        MethodType      m_MethodType;
        MediaType       m_Produces;
        MediaType       m_Consumes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="route">Route</param>
        /// <param name="Method">Method</param>
        /// <param name="produces">Produces</param>
        public EndPointAttribute(String route, String method = "GET", 
                                 MediaType consumes=MediaType.APPLICATION_OCTET_STREAM, 
                                 MediaType produces = MediaType.APPLICATION_OCTET_STREAM) : base (route)
        {
            switch(method)
            {
                case "GET":
                    m_MethodType = MethodType.GET;
                    break;
            }
            
            m_Produces = produces;
            m_Consumes = consumes;
        }

        internal MethodType Method
        {
            get { return m_MethodType; }
        }

        internal MediaType Produces
        {
            get { return m_Produces; }
        }

        internal MediaType Consumes
        {
            get { return m_Consumes; }
        }
    }
}
