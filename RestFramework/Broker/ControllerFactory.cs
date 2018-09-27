using System;
using System.Collections.Generic;

using System.Text;
using System.Text.RegularExpressions;

using System.Reflection;
using RestFramework.Annotations;
using RestFramework.Helpers;

namespace RestFramework.Broker
{
    sealed class ControllerFactory
    {
        private Dictionary<String, ComponentMethodMapper> mMapOfGetControllers;
        private Dictionary<String, ComponentMethodMapper> mMapOfPostControllers;
        private Dictionary<String, ComponentMethodMapper> mMapOfPutControllers;

        public ControllerFactory()
        {
            mMapOfGetControllers = new Dictionary<string, ComponentMethodMapper>();
            mMapOfPostControllers = new Dictionary<string, ComponentMethodMapper>();
            mMapOfPutControllers = new Dictionary<string, ComponentMethodMapper>();
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

            switch (mthd)
            {
                case Method.GET:
                    foreach (String key in mMapOfGetControllers.Keys)
                    {
                        Regex parser = new Regex(key);
                        ret[1] = parser.IsMatch(URI) == true ? mMapOfGetControllers[key] : null;  
                        if (null != ret[1])
                            break;
                    }
                    break;
                case Method.POST:
                    ret[1] = mMapOfPostControllers.ContainsKey(URI) == true ? mMapOfPostControllers[URI] : null;
                    break;
                case Method.PUT:
                    ret[1] = mMapOfPutControllers.ContainsKey(URI) == true ? mMapOfPutControllers[URI] : null;
                    break;
            }

            return ret;
        }

        private void ScanAssemblyForAttributes(Assembly exA)
        {
            Type[] types = exA.GetTypes();

            foreach (Type T in types)
            {
                System.Console.WriteLine(T.Name);

                //detecting controller class 
                Object[] AttribOnClass = T.GetCustomAttributes(typeof(RouteAttribute), false);

                if (AttribOnClass.Length > 0)
                {
                    var obj = Activator.CreateInstance(T);

                    MemberInfo[] mInfo = T.GetMembers();

                    foreach (MemberInfo info in mInfo)
                    {
                        Attribute AttrbOnMethod = info.GetCustomAttribute(typeof(EndPointAttribute), false);
                       
                        if (null != AttrbOnMethod)
                        {
                            EndPointAttribute CntrlMthdAttr = (EndPointAttribute)AttrbOnMethod;
                            Dictionary<String,ComponentMethodMapper> refHandle = null; 
                            switch (CntrlMthdAttr.Method)
                            {
                                case Method.GET:
                                    refHandle = mMapOfGetControllers;
                                    break;
                                case Method.POST:
                                    refHandle = mMapOfPostControllers;
                                    break;
                                case Method.PUT:
                                    refHandle = mMapOfPutControllers;
                                    break;
                                default:
                                    throw new Exception();

                            }

                            //split the CntrlMthdAttr route to take only the context path 
                            //void of path variables
                            Regex parser = new Regex("[a-z A-z 0-9]*{[a-z A-z 0-9]*}");
                            String[] str = parser.Split(CntrlMthdAttr.Route);
                            StringBuilder bldr = new StringBuilder();
                            for (int i = 0; i < str.Length; ++i)
                            {
                                bldr.Append(str[i]);
                                if (i < str.Length - 1)
                                    bldr.Append(".*");
                            }

                            String mapperKey = ((RouteAttribute)AttribOnClass[0]).Route +
                                                bldr.ToString();

                            var hdlr = new ComponentMethodMapper();
                            hdlr.AddMethodDetails(obj, info as MethodInfo, 
                                ((RouteAttribute)AttribOnClass[0]).Route + CntrlMthdAttr.Route );

                            refHandle.Add(mapperKey, hdlr);
                        }
                    }
                }
            }
        }
    }
}
