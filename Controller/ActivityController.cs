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
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet("/api/activity/list")]
        public async Task<IActionResult> GetActivityList()
        {
            return Ok(await _activityService.GetActivityList());
        }
    }
}