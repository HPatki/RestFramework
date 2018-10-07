using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.Serialization;

using RestFramework;

namespace RestApplication
{
    [DataContract]
    class UserInfo
    {
        [DataMember]
        String name;
        [DataMember]
        String password;
    }

    class Program
    {
        static void Main(string[] args)
        {
            RestFramework.Program.createFactories();
            RestFramework.Program.createServer();

        }
    }
}
