using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HttpdServer.Helpers;
using RestFramework.Broker;
using RestFramework.Annotations;

namespace UnitTests
{

    [TestClass]
    public class UnitTest1
    {
        [RouteAttribute("test")]
        public class Test
        {
            [EndPointAttribute("/method1/{userParam}?QueryParam=")]
            public void testMethod1([BodyQueryParam("User")] String user, [PathVariable("userParam")]String usrParam)
            {
                System.Console.WriteLine("Hello User" + user);
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            RestFramework.Program.createFactories();
        }
    }
}
