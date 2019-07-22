using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;

namespace DemoCode {
    class Program {

        static ElasticClient _client;
        static async Task Main (string[] args) {
            InitializeClient ();
            await AddPeopleAsync (BuildTestData());

            await Task.Delay(Timeout.Infinite);
        }

        static void InitializeClient () {
            var uris = new Uri[] { new Uri ("http://172.17.78.111:9200"), new Uri ("http://172.17.78.112:9200") }; //支持多个节点
            var connectionPool = new SniffingConnectionPool (uris);
            var settings = new ConnectionSettings (connectionPool).DefaultIndex ("testindex");
            settings.BasicAuthentication ("", ""); //设置账号密码
            _client = new ElasticClient (settings);
        }

        static async Task AddPeopleAsync (People[] peoples) {
            foreach (var p in peoples) {
                var response=await _client.IndexDocumentAsync (p);
            }
        }
        static People[] BuildTestData () {
            return new People[] {
                new People { Id = Guid.NewGuid (), Name = "张三", Age = 24, Birthday = new DateTime (1984, 12, 4), Address = "上海市长宁区", Gender = true },
                new People { Id = Guid.NewGuid (), Name = "李四", Age = 24, Birthday = new DateTime (1989, 6, 23), Address = "上海市普陀区", Gender = false },
                new People { Id = Guid.NewGuid (), Name = "王五", Age = 24, Birthday = new DateTime (1993, 9, 7), Address = "上海市静安区", Gender = false },
                new People { Id = Guid.NewGuid (), Name = "赵六", Age = 24, Birthday = new DateTime (1967, 1, 28), Address = "上海市浦东新区", Gender = true },
                new People { Id = Guid.NewGuid (), Name = "阮小七", Age = 24, Birthday = new DateTime (1988, 1, 6), Address = "上海市闵行区", Gender = true },
            };
        }
    }

    public class People {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
        public bool Gender { get; set; }
        public string Address { get; set; }
    }
}