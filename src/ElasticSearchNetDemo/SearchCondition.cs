using System;

namespace ElasticSearchNetDemo
{
    public class SearchCondition {
        public string Name { get; set; }
        public int? Age { get; set; }
        public bool? Gender { get; set; }
        public string Address { get; set; }
        public DateTime BeginCreateTime { get; set; }
        public DateTime EndCreateTime { get; set; }
    }
}