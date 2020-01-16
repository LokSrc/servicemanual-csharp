using System;

namespace EtteplanMORE.ServiceManual.ApplicationCore.Entities
{
    public class ServiceTaskDto
    {
        public int TaskId { get; set; }
        public int TargetId { get; set; }
        public FactoryDeviceDto Target { get; set;}
        public TaskCriticality Criticality { get; set; }
        public DateTime DateIssued { get; set; }
        public string Description { get; set; }
        public bool Closed { get; set; }
    }
}
