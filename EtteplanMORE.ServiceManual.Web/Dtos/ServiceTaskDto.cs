using System;

namespace EtteplanMORE.ServiceManual.ApplicationCore.Entities
{
    public class ServiceTaskDto
    {
        public FactoryDevice Target { get; set; }
        public Criticality Critical { get; set; }
        public DateTime DateIssued { get; set; }
        public string Description { get; set; }
        public bool Closed { get; set; }
    }
}
