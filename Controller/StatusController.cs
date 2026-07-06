using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inventory_v2.Services;
using Microsoft.AspNetCore.Mvc;

namespace inventory_v2.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly IStatusService _statusService;

        public StatusController(IStatusService statusService)
        {
            _statusService = statusService;
        }
        [HttpGet("/api/status/list")]
        public async Task<IActionResult> GetStatusList()
        {
            return Ok(await _statusService.GetStatusList());
        }

        [HttpPost("/api/status/add")]
        public async Task<IActionResult> AddStatus([FromBody] Model.Status status){
            return Ok(await _statusService.AddStatus(status));
        }
    }
}