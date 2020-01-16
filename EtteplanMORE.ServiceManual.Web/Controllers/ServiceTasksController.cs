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

            // Add target object to every task
            FactoryDevice dev;
            foreach (ServiceTaskDto task in tasks)
            {
                dev = await _factoryDeviceService.Get(task.TargetId);
                task.Target = new FactoryDeviceDto
                {
                    Id = dev.Id,
                    Name = dev.Name,
                    Type = dev.Type,
                    Year = dev.Year
                };
            }
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
                Target = new FactoryDeviceDto
                {
                    Id = dev.Id,
                    Name = dev.Name,
                    Type = dev.Type,
                    Year = dev.Year
                }
            });
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
                task.Criticality : TaskCriticality.Mild,
                DateIssued = DateTime.Now,
                Description = task.Description != "" ? 
                task.Description : "No description provided.",
                TargetId = task.TargetId,
                TaskId = (await _serviceTaskService.GetAllAsync()).Count()
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
            ServiceTask updateData = new ServiceTask
            {
                Closed = UpdateData.Closed,
                Criticality = UpdateData.Criticality,
                Description = UpdateData.Description,
                TargetId = UpdateData.TargetId,
                TaskId = id
            };
            await _serviceTaskService.UpdateAsync(updateData);

            return Ok(Json("Edit succesful."));
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
            return Ok(Json("Service task removed."));
        }

    }
}



/* Default values:
 * TaskId: next open
 * TargetId: is needed always!!!
 * Criticality: critical
 * DateIssued: Current time
 * Description: "No description provided."
 * Closed: false
 */

/* Can be edited
 * TaskId: NO
 * TargetId: Yes
 * Criticality: Yes
 * DateIssued: No
 * Description: Yes
 * Closed: YES NOTE: Not giving this will set it to false
 */


// GET /api/servicetasks                LISTAUS // DONE
// GET /api/servicetasks/target/id      SUODATETTU LISTAUS // DONE
// GET /api/servicetasks? Params        HAKU // TODO
// POST /api/servicetasks? Params       LISÄYS // DONE
// PUT /api/servicetasks/id? Params     MUOKKAUS // DONE, NOT TESTED AT ALL CHECK INPUT ORDER (int id, UpdateData)...
// DELETE /api/servicetasks/id          POISTO // DONE
