using System;
using System.Collections.Generic;

using System.Text;
using System.Text.RegularExpressions;

using System.Reflection;
using RestFramework.Annotations;
using HttpdServer.Helpers;
using RestFramework.Broker;
using RestFramework.Exceptions;

namespace RestFramework.Factories
{
    sealed class ControllerFactory
    {
        private Dictionary<String, ComponentMethodMapper> m_MapOfGetControllers;
        private Dictionary<String, ComponentMethodMapper> m_MapOfPostControllers;
        private Dictionary<String, ComponentMethodMapper> m_MapOfPutControllers;
        private Dictionary<String, ComponentMethodMapper> m_EmptyController;
        public ControllerFactory()
        {
            m_MapOfGetControllers = new Dictionary<string, ComponentMethodMapper>();
            m_MapOfPostControllers = new Dictionary<string, ComponentMethodMapper>();
            m_MapOfPutControllers = new Dictionary<string, ComponentMethodMapper>();
            m_EmptyController = new Dictionary<string, ComponentMethodMapper>();
        }

        public void ConstructSingleTons()
        {
            AppDomain defaultDomain = AppDomain.CurrentDomain;
            Assembly[] loadedAssemblies = defaultDomain.GetAssemblies();

            foreach (Assembly assmbly in loadedAssemblies)
            {
                //retrieve classes with given attribute and form the map
                ScanAssemblyForAttributes(assmbly);

                //foreach (System.Reflection.AssemblyName AN in System.Reflection.Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                //{
                //    System.Reflection.Assembly A = System.Reflection.Assembly.Load(AN.ToString());
                //    ScanAssemblyForAttributes(A);
                //}
            }
        }

        public object[] getMethodMapper(Method mthd, String URI)
        {
            object[] ret = new Object[2];
            Dictionary<String, ComponentMethodMapper> container = null;

            switch (mthd)
            {
                case Method.GET:
                    container = m_MapOfGetControllers;
                    break;
                case Method.POST:
                    container = m_MapOfPostControllers;
                    break;
                case Method.PUT:
                    container = m_MapOfPutControllers;
                    break;
                default:
                    container = m_EmptyController;
                    break;
                    
            }

            foreach (String key in container.Keys)
            {
                //[Fixed bug :: the complete key has to match and no partial match
                //should happen
                Regex parser = new Regex("^"+key+"$");

                ret[1] = parser.IsMatch(URI) == true ? container[key] : null;
                if (null != ret[1])
                    break;
            }

            return ret;
        }

        private void ScanAssemblyForAttributes(Assembly exA)
        {
            Type[] types = exA.GetTypes();

            foreach (Type T in types)
            {
                //System.Console.WriteLine(T.Name);

                //detecting controller class 
                Object[] AttribOnClass = T.GetCustomAttributes(typeof(RouteAttribute), false);

                if (AttribOnClass.Length > 0)
                {
                    var obj = Activator.CreateInstance(T);

                    MethodInfo[] mInfo = T.GetMethods();

                    foreach (MethodInfo info in mInfo)
                    {
                        Attribute AttrbOnMethod = info.GetCustomAttribute(typeof(EndPointAttribute), false);
                       
                        if (null != AttrbOnMethod)
                        {
                            EndPointAttribute CntrlMthdAttr = (EndPointAttribute)AttrbOnMethod;
                            Dictionary<String,ComponentMethodMapper> refHandle = null; 
                            switch (CntrlMthdAttr.Method)
                            {
                                case Method.GET:
                                    refHandle = m_MapOfGetControllers;
                                    break;
                                case Method.POST:
                                    refHandle = m_MapOfPostControllers;
                                    break;
                                case Method.PUT:
                                    refHandle = m_MapOfPutControllers;
                                    break;
                                default:
                                    throw new Exception();

                            }

                            //split the CntrlMthdAttr route to take only the context path 
                            //void of path variables
                            Regex parser = new Regex("[a-z A-z 0-9]*{[a-z A-z 0-9]*}");
                            String[] str = parser.Split(CntrlMthdAttr.getName());
                            StringBuilder bldr = new StringBuilder();
                            for (int i = 0; i < str.Length; ++i)
                            {
                                bldr.Append(str[i]);
                                if (i < str.Length - 1)
                                    bldr.Append(".*");
                            }

                            String mapperKey = ((RouteAttribute)AttribOnClass[0]).getName() +
                                                bldr.ToString();

                            var hdlr = new ComponentMethodMapper();
                            hdlr.AddMethodDetails(obj, info as MethodInfo, 
                                ((RouteAttribute)AttribOnClass[0]).getName() + CntrlMthdAttr.getName(),
                                CntrlMthdAttr);

                            try
                            {
                                refHandle.Add(mapperKey, hdlr);
                            }
                            catch (ArgumentException err)
                            {
                                throw new SameEndPointURL(((RouteAttribute)AttribOnClass[0]).getName() + CntrlMthdAttr.getName()); 
                            }
                        }
                    }
                }
            }
        }
    }
}
