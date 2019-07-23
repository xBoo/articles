using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ElasticSearchNetDemo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ElasticSearchDemoTest
{
    [TestClass]
    public class ElasticServiceTest
    {
        private ElasticService _elasticService;

        [TestInitialize]
        public void TestInit()
        {
            this._elasticService = new ElasticService();
        }

        [TestMethod]
        public async Task AddPeopleTest()
        {
            var peoples = this.BuildTestData();
            var response = await this._elasticService.AddPeopleAsync(peoples);
            Assert.IsTrue(response.Items.All(a => a.IsValid));
        }

        [TestMethod]
        public async Task QueryPeopleTest()
        {
            var condition = new SearchCondition
            {
                Address="长宁区",
                BeginCreateTime = DateTime.Now.AddDays(-1),
                EndCreateTime = DateTime.Now
            };

            var result = await this._elasticService.QueryPeopleAsync(condition, 0, 3);
            Assert.IsTrue(result.IsSuccess);
        }
        private People[] BuildTestData()
        {
            return new People[] {
                new People { Id = Guid.NewGuid (), Name = "张三", Age = 24, Birthday = new DateTime (1984, 12, 4), Address = "上海市长宁区", Gender = true },
                new People { Id = Guid.NewGuid (), Name = "李四", Age = 24, Birthday = new DateTime (1989, 6, 23), Address = "上海市普陀区", Gender = false },
                new People { Id = Guid.NewGuid (), Name = "王五", Age = 24, Birthday = new DateTime (1993, 9, 7), Address = "上海市静安区", Gender = false },
                new People { Id = Guid.NewGuid (), Name = "赵六", Age = 24, Birthday = new DateTime (1967, 1, 28), Address = "上海市浦东新区", Gender = true },
                new People { Id = Guid.NewGuid (), Name = "阮小七", Age = 24, Birthday = new DateTime (1988, 1, 6), Address = "上海市闵行区", Gender = true },
            };
        }
    }
}

