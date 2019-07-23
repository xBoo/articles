using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using System.Linq;

namespace ElasticSearchNetDemo
{
    public class ElasticService
    {
        private readonly ElasticClient _client;

        public ElasticService()
        {
            var uris = new Uri[] { new Uri("http://172.17.78.111:9200"), new Uri("http://172.17.78.112:9200") }; //支持多个节点
            var connectionPool = new SniffingConnectionPool(uris);
            var settings = new ConnectionSettings(connectionPool).DefaultIndex("testindex");
            settings.BasicAuthentication("", ""); //设置账号密码
            this._client = new ElasticClient(settings);
        }

        public async Task<IBulkResponse> AddPeopleAsync(People[] peoples)
        {
            var descriptor = new BulkDescriptor();
            foreach (var p in peoples)
            {
                var response = await _client.IndexDocumentAsync(p);
                descriptor.Index<People>(op => op.Document(p));

            }

            return await _client.BulkAsync(descriptor);
        }


        public async Task<PagedResult<People[]>> QueryPeopleAsync(SearchCondition condition, int pageIndex,
            int pageSize)
        {
            var query = this.BuildQueryContainer(condition);
            var response = await this._client.SearchAsync<People>(s => s
                  .Index("testindex")
                  .From(pageIndex * pageSize)
                  .Size(pageSize)
                  .Query(q => query)
                  .Sort(st => st.Descending(d => d.CreateTime)));

            if (response.ApiCall.Success)
            {
                return new PagedResult<People[]>
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    Total = response.Total,
                    ReturnObj = response.Hits.Select(s => s.Source).ToArray()
                };
            }

            return new PagedResult<People[]> { IsSuccess = false };
        }

        public QueryContainer BuildQueryContainer(SearchCondition condition)
        {
            var queryCombin = new List<Func<QueryContainerDescriptor<People>, QueryContainer>>();
            if (!string.IsNullOrEmpty(condition.Name))
                queryCombin.Add(mt => mt.Match(m => m.Field(t => t.Name).Query(condition.Name))); //字符串匹配

            if (condition.Age.HasValue)
                queryCombin.Add(mt => mt.Range(m => m.Field(t => t.Address).GreaterThanOrEquals(condition.Age))); //数值区间匹配

            if (!string.IsNullOrEmpty(condition.Address))
                //短语匹配
                queryCombin.Add(mt => mt.MatchPhrase(m => m.Field(t => t.Address).Query(condition.Address))); 

            if (!condition.Gender.HasValue)
                queryCombin.Add(mt => mt.Term(m => m.Field(t => t.Gender).Value(condition.Gender)));//精确匹配

            return Query<People>.Bool(b => b
               .Must(queryCombin)
               .Filter(f => f
                  .DateRange(dr => dr.Field(t => t.CreateTime)
                     .GreaterThanOrEquals(DateMath.Anchored(condition.BeginCreateTime.ToString("yyyy-MM-ddTHH:mm:ss")))
                     .LessThanOrEquals(DateMath.Anchored(condition.EndCreateTime.ToString("yyyy-MM-ddTHH:mm:ss"))))));
        }
    }

    public class PagedResult<T>
    {
        public bool IsSuccess { get; set; } = true;
        public long PageSize { get; set; }
        public long PageIndex { get; set; }
        public long Total { get; set; }
        public T ReturnObj { get; set; }
    }
}
