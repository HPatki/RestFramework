using System;
using System.Collections.Generic;

using System.Text;
using System.Net;
using System.Net.Http;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                HttpClient client = new HttpClient();
                var response = client.PostAsync("http://localhost:15990/basic/getName/harshad/dummy/patki?salutation=Mr&middlename=suhas", null);
                //response.Start();
                while (response.IsCompleted == false)
                {

                }

                HttpResponseMessage msg = response.Result;
                if (msg.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent cont = msg.Content;
                    var str = cont.ReadAsStringAsync();
                    while (str.IsCompleted == false)
                    {

                    }

                    System.Console.WriteLine(str.Result);
                }
                else
                {
                    System.Console.WriteLine("Error");
                }
            }
        }
    }
}
