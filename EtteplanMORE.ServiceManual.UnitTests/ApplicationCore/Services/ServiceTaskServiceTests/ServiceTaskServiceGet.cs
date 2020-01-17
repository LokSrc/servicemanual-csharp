using System;
using System.Linq;
using EtteplanMORE.ServiceManual.ApplicationCore.Entities;
using EtteplanMORE.ServiceManual.ApplicationCore.Interfaces;
using EtteplanMORE.ServiceManual.ApplicationCore.Services;
using Xunit;

namespace EtteplanMORE.ServiceManual.UnitTests.ApplicationCore.Services.ServiceTaskServiceTests
{
    public class ServiceTaskServiceGet
    {
        [Fact]
        public async void AllTasks()
        {
            IServiceTaskService serviceTaskService = new ServiceTaskService();

            var sts = (await serviceTaskService.GetAllAsync()).ToList();

            Assert.NotNull(sts);
            Assert.NotEmpty(sts);
            Assert.True(10 <= sts.Count());
        }

        [Fact]
        public async void ExistingTasksForTarget()
        {
            IServiceTaskService serviceTaskService = new ServiceTaskService();
            int fdId = 11;

            var sts = (await serviceTaskService.GetAsync(fdId)).ToList();

            Assert.NotNull(sts);
            Assert.Equal(2, sts.Count);
        }

        [Fact]
        public async void NonExistingTasksForTarget()
        {
            IServiceTaskService serviceTaskService = new ServiceTaskService();
            int fdId = 1;

            var sts = (await serviceTaskService.GetAsync(fdId)).ToList();

            Assert.Empty(sts);
        }

        [Fact]
        public async void SearchByCriticality()
        {
            Search search = new Search
            {
                MinCriticality = 1
            };
            IServiceTaskService serviceTaskService = new ServiceTaskService();

            var sts = (await serviceTaskService.SearchAsync(search)).ToList();

            foreach (var st in sts)
            {
                Assert.Equal(1, (int)st.Criticality);
            }
        }
    }
}
