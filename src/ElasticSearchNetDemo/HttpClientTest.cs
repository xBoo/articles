using System;
using System.Net;
using System.Threading.Tasks;

namespace ElasticSearchNetDemo
{
    public class HttpClientTest
    {
        public void Run(){
            Task[] tasks = new Task[10];
            for (int i = 0; i < 10; i++) {
                Console.WriteLine (i);
                tasks[i] = Task.Run (() => {
                    for (int j = 0; j < 10000; j++) {
                        var req = CreateRequest ("Get", "Http://www.baidu.com");
                        var response = req.GetResponse ();
                    }
                });
            }
            Task.WaitAll (tasks);
            Console.WriteLine ("Hello,world!");
            Console.ReadKey ();
        }

        private HttpWebRequest CreateRequest (string method, string uri) {
            var request = (HttpWebRequest) WebRequest.Create (uri);
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.Timeout = 60000;
            request.Method = method;

            return request;
        }
    }
}