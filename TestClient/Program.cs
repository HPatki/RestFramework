using System;
using System.Collections.Generic;

using System.Text;
using System.Net;
using System.Net.Http;
using System.Diagnostics;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            String[] urls = new String[2];
            urls[0] = "http://localhost:15990/admin/login";
            urls[1] = "http://localhost:14990/admin/login";
            int i = 0;

            while (true)
            {
                HttpClient client = new HttpClient();
                String data = "{\"user\":\"harshad\",\"passwd\":\"Passwd1$\"}";
                var payload = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(data));
                payload.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                payload.Headers.ContentLength = data.Length;
                Stopwatch wtch = Stopwatch.StartNew();
                var response = client.PostAsync(urls[i], payload);
                i = i == 0 ? 1 : 0;
                //response.Start();
                while (response.IsCompleted == false)
                {

                }

                if (response.IsFaulted == false)
                {
                    wtch.Stop();
                    System.Console.WriteLine ("Time :: " + wtch.ElapsedMilliseconds + " i " + i);
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
                        System.Console.WriteLine("Status was :: " + msg.StatusCode);
                    }
                }
                else
                {
                    
                    wtch.Stop();
                    System.Console.WriteLine ("Time :: " + wtch.ElapsedMilliseconds);
                    System.Console.WriteLine("Error");
                }
            }
        }
    }
}
