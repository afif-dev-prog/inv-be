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
    public class ChildAssetsController : ControllerBase
    {
        private readonly IChildAssetsService _childAssetsService;
        private readonly IAssetsService _assetsService;

        public ChildAssetsController(IChildAssetsService childAssetsService, IAssetsService assetsService)
        {
            _childAssetsService = childAssetsService;
            _assetsService = assetsService;
        }

        [HttpGet("/api/childassets/list")]
        public async Task<IActionResult> GetChildAssetsList()
        {
            return Ok(await _childAssetsService.GetChildAssetsList());
        }

        [HttpPost("/api/childassets/add")]
        public async Task<IActionResult> AddChildAsset(string id, [FromBody] Model.ChildAsset childAsset)
        {
            return Ok(await _childAssetsService.AddChildAsset(id, childAsset));
        }

        [HttpPut("/api/childassets/update/{id}")]
        public async Task<IActionResult> UpdateChildAsset([FromRoute] string id, [FromBody] Model.ChildAsset updatedChildAsset)
        {
            return Ok(await _childAssetsService.UpdateChildAsset(id, updatedChildAsset));
        }

        [HttpDelete("/api/childassets/delete/{id}")]
        public async Task<IActionResult> DeleteChildAsset([FromRoute] string id)
        {
            return Ok(await _childAssetsService.DeleteChildAsset(id));
        }
    }
}