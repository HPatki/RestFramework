using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestFramework.Annotations;

namespace RestFramework.Interface
{
    [RouteAttribute ("test")]
    public class Test
    {
        [EndPointAttribute("/method1/{userParam}?QueryParam=")]
        public void testMethod1([BodyQueryParam("User")] String user, [PathVariable("userParam")]String usrParam)
        {
            System.Console.WriteLine ("Hello User" + user);
        }
    }
}
