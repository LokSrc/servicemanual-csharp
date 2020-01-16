using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using EtteplanMORE.ServiceManual.ApplicationCore.Entities;
using EtteplanMORE.ServiceManual.ApplicationCore.Interfaces;

namespace EtteplanMORE.ServiceManual.ApplicationCore.Services
{
    public class ServiceTaskService : IServiceTaskService
    {
        public Task<ServiceTask> Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ServiceTask>> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
