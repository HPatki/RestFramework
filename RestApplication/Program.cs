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
            object vall = new Program();
            Type T = new String('&',1).GetType();
            var vallC = Convert.ChangeType(vall, vall.GetType());

            Regex parser = new Regex("[a-z A-z 0-9]*{[a-z A-z 0-9]*}");
            String[] str = parser.Split("/admin/login/{name}/dummy/{surname}?middle=suhas");
            StringBuilder bldr = new StringBuilder();
            for (int i=0; i<str.Length;++i)
            {
                bldr.Append(str[i]);
                if ( i < str.Length -1)
                    bldr.Append(".*");
            }
            Regex parser2 = new Regex(bldr.ToString());
            Boolean matched = parser2.IsMatch("/admin/login/harshad/dummy/patki?middle=suhas");
            MatchCollection coll = parser.Matches("/admin/login/{name}/dummy/{surname}?middle=suhas");

            foreach (String oneStr in str)
            {
                if (oneStr.Contains("}"))
                {
                    String[] splits = oneStr.Split('}');
                }
            }
            //GroupCollection coll = str.Groups;
            //foreach (Group grp in coll)
            //{
            //    System.Console.WriteLine(grp.Value);
            //}
            char[] seps = {'{','}'};
            String[] splitval = "/b/c/{name}/l/{surname}".Split(seps);

            RestFramework.Program.createFactories();
            RestFramework.Program.createServer();

        }
    }
}
