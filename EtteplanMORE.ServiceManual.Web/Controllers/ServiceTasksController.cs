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
        private readonly IFactoryDeviceService _factoryDeviceService;

        public ServiceTasksController(IServiceTaskService serviceTaskService, 
            IFactoryDeviceService factoryDeviceService)
        {
            _serviceTaskService = serviceTaskService;
            _factoryDeviceService = factoryDeviceService;
        }

        private async Task AddTargets(List<ServiceTaskDto> Tasks)
        {
            // Add target object to every task
            FactoryDevice dev;
            foreach (ServiceTaskDto task in Tasks)
            {
                dev = await _factoryDeviceService.Get(task.TargetId);

                // If target not found. (These tasks should be removed/edited)
                if (dev == null)
                {
                    task.Target = null;
                    continue;
                }

                task.Target = new FactoryDeviceDto
                {
                    Id = dev.Id,
                    Name = dev.Name,
                    Type = dev.Type,
                    Year = dev.Year
                };
            }
        }

        /// <summary>
        ///     List all
        ///     HTTP GET: api/servicetasks/
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<ServiceTaskDto>> Get()
        {
             // List all tasks 
             List<ServiceTaskDto> tasks = (await _serviceTaskService.GetAllAsync())
                .Select(st => new ServiceTaskDto
                {
                    TaskId = st.TaskId,
                    Closed = st.Closed,
                    Criticality = st.Criticality,
                    DateIssued = st.DateIssued,
                    Description = st.Description,
                    TargetId = st.TargetId
                }).ToList();

            await AddTargets(tasks);
            return tasks;
        }

        /// <summary>
        ///     List by device {id}
        ///     HTTP GET: api/servicetasks/target/{id}
        /// </summary>
        [HttpGet("target/{id}")]
        public async Task<IEnumerable<ServiceTaskDto>> Get(int id)
        {
            // Target object for response
            FactoryDevice dev = await _factoryDeviceService.Get(id);
            return (await _serviceTaskService.GetAsync(id))
                .Select(st => new ServiceTaskDto
            {
                TaskId = st.TaskId,
                Closed = st.Closed,
                Criticality = st.Criticality,
                DateIssued = st.DateIssued,
                Description = st.Description,
                TargetId = st.TargetId,
                Target = dev != null ? new FactoryDeviceDto
                {
                    Id = dev.Id,
                    Name = dev.Name,
                    Type = dev.Type,
                    Year = dev.Year
                } : null
            });
        }

        /// <summary>
        ///     Search with params
        ///     HTTP GET: /api/servicetasks/search?{PARAMS}
        /// </summary>
        /// <param name="SearchData"></param>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<IEnumerable<ServiceTaskDto>> Get(SearchDto SearchData)
        {
            List<ServiceTaskDto> tasks = (await _serviceTaskService.SearchAsync(SearchData))
                .Select(st => new ServiceTaskDto
                {
                    TaskId = st.TaskId,
                    Closed = st.Closed,
                    Criticality = st.Criticality,
                    DateIssued = st.DateIssued,
                    Description = st.Description,
                    TargetId = st.TargetId
                }).ToList();

            await AddTargets(tasks);
            return tasks;
        }
        
        /// <summary>
        ///     Handle post request used to add servicetasks.
        ///     HTTP POST: api/servicetasks?{PARAMS}
        /// </summary>
        /// <param name="task">Request parameters</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(ServiceTaskDto task)
        {
            if (task.TargetId == 0)
            {
                return BadRequest(Json("Error: TargetId must be provided."));
            }

            if (await _factoryDeviceService.Get(task.TargetId) == null)
            {
                return NotFound(Json("Error: Target for task was not found."));
            }

            ServiceTask newTask = new ServiceTask
            {
                Closed = task.Closed,
                Criticality = task.Criticality != 0 ?
                task.Criticality : TaskCriticality.Mild, // Default is Mild
                DateIssued = DateTime.Now,
                Description = task.Description ?? "No description provided.",
                TargetId = task.TargetId
            };

            await _serviceTaskService.CreateAsync(newTask);
            return Ok(Json("Service task created."));
        }

        /// <summary>
        ///     Handle put request used to edit servicetask.
        ///     HTTP PUT: api/servicetasks/{id}?{PARAMS}
        /// </summary>
        /// <param name="UpdateData">Request params</param>
        /// <param name="id">TaskId</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(ServiceTaskDto UpdateData, int id)
        {   
            // If TargetId is provided it must be valid
            if (UpdateData.TargetId != 0)
            {
                FactoryDevice dev = await _factoryDeviceService.Get(UpdateData.TargetId);
                if (dev == null)
                {
                    return BadRequest(Json("Provided TargetId is invalid"));
                }
            }
            
            ServiceTask updateData = new ServiceTask
            {
                Closed = UpdateData.Closed,
                Criticality = UpdateData.Criticality,
                Description = UpdateData.Description,
                TargetId = UpdateData.TargetId,
                TaskId = id
            };
            await _serviceTaskService.UpdateAsync(updateData, id);
            return Ok(Json("Edit successful."));
        }

        /// <summary>
        ///     Handle delete request used to delete servicetasks.
        ///     HTTP DELETE: api/servicetasks/{id}
        /// </summary>
        /// <param name="id">Service task id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _serviceTaskService.DeleteAsync(id);
            return Ok(Json($"Service task with id:{id}" +
                $" is no longer in database"));
        }

    }
}
