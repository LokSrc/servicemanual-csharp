using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EtteplanMORE.ServiceManual.ApplicationCore.Entities;

namespace EtteplanMORE.ServiceManual.ApplicationCore.Interfaces
{
    public interface IServiceTaskService
    {
        Task<IEnumerable<ServiceTask>> GetAll();

        Task<ServiceTask> Get(int id);
    }
}
