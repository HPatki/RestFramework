using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.Serialization;

using RestFramework;
using RestApplication.controllers;
using HttpdServer;
using HttpdServer.Transport;

namespace RestApplication
{
    class Base
    {
        public void somefunc()
        {

        }
    }

    class Derived : Base
    {
        public void somefunc()
        {

        }
    }
    [DataContract]
    class UserInfo 
    {
        [DataMember]
        String name;
        [DataMember]
        String password;

        public UserInfo(Byte[] bytes)
        {

        }
    }


    [DataContract]
    class Greeting
    {
        [DataMember]
        public String name;
        [DataMember]
        public String surname;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Int32 port = 15990;
            String prt = port.ToString();
            SByte[] sprt = new SByte[prt.Length];
            for (int i = 0; i < prt.Length; ++i)
                sprt[i] = (sbyte)prt[i];
            RestFramework.Program.createFactories();
        }
    }
}
