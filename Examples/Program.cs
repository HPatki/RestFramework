﻿using System;
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
            RestFramework.Program.createFactories();
        }
    }
}
