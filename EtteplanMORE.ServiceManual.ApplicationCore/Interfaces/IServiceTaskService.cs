using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EtteplanMORE.ServiceManual.ApplicationCore.Entities;

namespace EtteplanMORE.ServiceManual.ApplicationCore.Interfaces
{
    public interface IServiceTaskService
    {
        Task<IEnumerable<ServiceTask>> GetAllAsync();

        Task<IEnumerable<ServiceTask>> GetAsync(int TargetId);

        Task<IEnumerable<ServiceTask>> SearchAsync(Search SearchData);

        Task<IAsyncResult> DeleteAsync(int TaskId);

        Task<IAsyncResult> UpdateAsync(ServiceTask UpdateData, int TaskId);

        Task<IAsyncResult> CreateAsync(ServiceTask task);
    }
}
