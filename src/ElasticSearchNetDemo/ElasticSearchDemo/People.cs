using System;

namespace ElasticSearchNetDemo
{
    public class People {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
        public bool Gender { get; set; }
        public string Address { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}