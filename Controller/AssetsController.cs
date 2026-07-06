using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inventory_v2.Model;
using inventory_v2.Services;
using Microsoft.AspNetCore.Mvc;

namespace inventory_v2.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetsService _assetsService;

        public AssetsController(IAssetsService assetsService)
        {
            _assetsService = assetsService;
        }

        [HttpGet("/api/assets/list")]
        public async Task<IActionResult> GetAssetsList()
        {
            return Ok(await _assetsService.GetAssetsList());
        }

        [HttpPost("/api/assets/add")]
        public async Task<IActionResult> AddAsset([FromBody] Assets asset)
        {
            return Ok(await _assetsService.AddAsset(asset));
        }

        [HttpPut("/api/assets/update/{id}")]
        public async Task<IActionResult> UpdateAsset([FromRoute] string id, [FromBody] Assets updatedAsset)
        {
            return Ok(await _assetsService.UpdateAsset(id, updatedAsset));
        }

        [HttpDelete("/api/assets/delete/{id}")]
        public async Task<IActionResult> DeleteAsset([FromRoute] string id)
        {
            return Ok(await _assetsService.DeleteAsset(id));
        }


        [HttpGet("/api/assets/movement-history/{id}")]
        public async Task<IActionResult> GetMovementHistoryById([FromRoute] string id)
        {
            return Ok(await _assetsService.GetMovementHistoryById(id));
        }
    }
}