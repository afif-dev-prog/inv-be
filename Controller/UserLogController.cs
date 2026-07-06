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
    public class UserLogController : ControllerBase
    {
        private readonly IUserLogService _userLogService;
        public UserLogController(IUserLogService userLogService)
        {
            _userLogService = userLogService;
        }

        [HttpGet("/api/userlog/list")]
        public async Task<IActionResult> GetUserLogList()
        {
            return Ok(await _userLogService.GetUserLogList());
        }

        [HttpPost("/api/userlog/add")]
        public async Task<IActionResult> AddUserLog([FromBody] Model.UserLog userLog)
        {
            return Ok(await _userLogService.AddUserLog(userLog));
        }
    }
}