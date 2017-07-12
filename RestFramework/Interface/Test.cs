using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestFramework.Annotations;

namespace RestFramework.Interface
{
    [ControllerAttribute ("test")]
    public class Test
    {
        [ControllerMethodAttribute("/method1/{userParam}?QueryParam=")]
        public void testMethod1([MethodBodyParam("User")] String user, [MethodRequestParam("userParam")]String usrParam)
        {

        }
    }
}
