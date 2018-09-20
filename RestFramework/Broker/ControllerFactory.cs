﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ComponentMethodMapper getMethodMapper(Method mthd, String URI)
        {
            return mMapOfGetControllers[URI];
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

                            var hdlr = new ComponentMethodMapper();
                            hdlr.AddMethodDetails(obj, info as MethodInfo);

                            refHandle.Add(((RouteAttribute)AttribOnClass[0]).Route + CntrlMthdAttr.Route,
                                        hdlr);

                            
                        }
                    }
                }
            }
        }
    }
}
