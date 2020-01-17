using System;

namespace EtteplanMORE.ServiceManual.ApplicationCore.Entities
{
    public class Search
    {
        public int TaskId { get; set; }
        public int TargetId { get; set; }
        public int MinCriticality { get; set; }
        public DateTime IssuedBefore { get; set; }
        public DateTime IssuedAfter { get; set; }
        public int Closed { get; set; }
        public string DescContains { get; set; }
    }
}
