using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EtteplanMORE.ServiceManual.ApplicationCore.Entities;
using EtteplanMORE.ServiceManual.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EtteplanMORE.ServiceManual.Web.Controllers
{
    [Route("api/[controller]")]
    public class ServiceTasksController : Controller
    {

        private readonly IServiceTaskService _serviceTaskService;

        public ServiceTasksController(IServiceTaskService serviceTaskService)
        {
            _serviceTaskService = serviceTaskService;
        }
        /// <summary>
        ///     HTTP GET: api/servicetasks/
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<ServiceTaskDto>> Get()
        {
            return (await _serviceTaskService.GetAll())
                .Select(st => new ServiceTaskDto
                {
                    // TODO: params
                });
        }

        /// <summary>
        ///     HTTP GET: api/servicetasks/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var st = await _serviceTaskService.Get(id);
            if (st == null)
            {
                return NotFound();
            }

            return Ok(new ServiceTaskDto
            {
                // TODO: params
            });
        }
    }
}