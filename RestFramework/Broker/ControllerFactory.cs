using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using RestFramework.Annotations;

namespace RestFramework.Broker
{
    class ControllerFactory
    {
        Dictionary<String, ComponentMethodMapper> mMapOfControllers;

        public ControllerFactory()
        {
            mMapOfControllers = new Dictionary<string, ComponentMethodMapper>();
        }

        public void ConstructSingleTons()
        {
            //retrieve classes with given attribute and form the map
            Assembly exA = System.Reflection.Assembly.GetExecutingAssembly();
            ScanAssemblyForAttributes(exA);

            foreach (System.Reflection.AssemblyName AN in System.Reflection.Assembly.GetExecutingAssembly().GetReferencedAssemblies())
            {
                System.Reflection.Assembly A = System.Reflection.Assembly.Load(AN.ToString());
                ScanAssemblyForAttributes(A);
            }
        }

        private void ScanAssemblyForAttributes(Assembly exA)
        {
            Type[] types = exA.GetTypes();

            foreach (Type T in types)
            {
                ComponentMethodMapper obj = null;
                System.Console.WriteLine(T.Name);

                Object[] AttribOnClass = T.GetCustomAttributes(typeof(ControllerAttribute), false);

                if (AttribOnClass.Length > 0)
                {
                    MemberInfo[] mInfo = T.GetMembers();

                    foreach (MemberInfo info in mInfo)
                    {
                        Attribute AttrbOnMethod = info.GetCustomAttribute(typeof(ControllerMethodAttribute), false);
                       
                        if (null != AttrbOnMethod)
                        {
                            ControllerMethodAttribute CntrlMthdAttr = (ControllerMethodAttribute)AttrbOnMethod;

                            if (null == obj)
                            {
                                mMapOfControllers.Add(((ControllerAttribute)AttribOnClass[0]).Route, obj=new ComponentMethodMapper(T));
                            }

                            obj.AddMethodDetails(CntrlMthdAttr, info);
                            
                        }
                    }
                }
            }
        }
    }
}
