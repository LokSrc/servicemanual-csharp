using System;
using System.Linq;
using EtteplanMORE.ServiceManual.ApplicationCore.Interfaces;
using EtteplanMORE.ServiceManual.ApplicationCore.Services;
using Xunit;

namespace EtteplanMORE.ServiceManual.UnitTests.ApplicationCore.Services.FactoryDeviceServiceTests
{
    public class FactoryDeviceGet
    {
        [Fact]
        public async void AllDevices()
        {
            IFactoryDeviceService factoryDeviceService = new FactoryDeviceService();

            var fds = (await factoryDeviceService.GetAll()).ToList();

            Assert.NotNull(fds);
            Assert.NotEmpty(fds);
            Assert.Equal(30, fds.Count);
        }

        [Fact]
        public async void ExistingDeviceWithId()
        {
            IFactoryDeviceService FactoryDeviceService = new FactoryDeviceService();
            int fdId = 1;

            var fd = await FactoryDeviceService.Get(fdId);

            Assert.NotNull(fd);
            Assert.Equal(fdId, fd.Id);
        }

        [Fact]
        public async void NonExistingDeviceWithId()
        {
            IFactoryDeviceService FactoryDeviceService = new FactoryDeviceService();
            int fdId = 100;

            var fd = await FactoryDeviceService.Get(fdId);

            Assert.Null(fd);
        }
    }
}