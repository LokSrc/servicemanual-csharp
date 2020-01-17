using System;
using System.Linq;
using EtteplanMORE.ServiceManual.ApplicationCore.Entities;
using EtteplanMORE.ServiceManual.ApplicationCore.Interfaces;
using EtteplanMORE.ServiceManual.ApplicationCore.Services;
using Xunit;

namespace EtteplanMORE.ServiceManual.UnitTests.ApplicationCore.Services.ServiceTaskServiceTests
{
    public class ServiceTaskServiceEdit
    {
        [Fact]
        public async void UpdateOne()
        {
            int taskid = 1;
            ServiceTask UpdateData = new ServiceTask
            {
                TargetId = 1,
                Description = "Edited",
                Closed = true
            };
            IServiceTaskService serviceTaskService = new ServiceTaskService();

            await serviceTaskService.UpdateAsync(UpdateData, taskid);

            var sts = (await serviceTaskService.GetAllAsync()).ToList();

            foreach (var st in sts)
            {
                if (st.TaskId == taskid)
                {
                    Assert.Equal(1, st.TargetId);
                    Assert.Equal("Edited", st.Description);
                    Assert.True(st.Closed);
                }
            }
        }

        [Fact]
        public async void DeleteOne()
        {
            int taskid = 1;
            IServiceTaskService serviceTaskService = new ServiceTaskService();

            await serviceTaskService.DeleteAsync(taskid);

            var sts = (await serviceTaskService.GetAllAsync()).ToList();

            foreach (var st in sts)
            {
                Assert.NotEqual(1, st.TaskId);
            }
        }

        [Fact]
        public async void CreateOne()
        {
            ServiceTask newTask = new ServiceTask
            {
                Closed = false,
                TargetId = 13,
                Description = "Find this",
                Criticality = TaskCriticality.Critical
            };
            IServiceTaskService serviceTaskService = new ServiceTaskService();

            await serviceTaskService.CreateAsync(newTask);

            var sts = (await serviceTaskService.GetAsync(13)).ToList();

            Assert.NotEmpty(sts);
            foreach (var st in sts)
            {
                if (st.Description == "Find this")
                {
                    Assert.False(st.Closed);
                    Assert.Equal(TaskCriticality.Critical, st.Criticality);
                    return;
                }   
            }
            Assert.Equal(0, 1);
        }
    }
}
