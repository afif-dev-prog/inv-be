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
    public class TypesController : ControllerBase
    {
        private readonly ITypesService _typesService;

        public TypesController(ITypesService typesService)
        {
            _typesService = typesService;
        }

        [HttpGet("/api/types/list")]
        public async Task<IActionResult> GetTypeList()
        {
            return Ok(await _typesService.GetTypeList());
        }

        [HttpPost("/api/types/add")]
        public async Task<IActionResult> AddType([FromBody] Model.Types type){
            return Ok(await _typesService.AddType(type));
        }
    }
}