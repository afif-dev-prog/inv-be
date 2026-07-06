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
    public class MovementController : ControllerBase
    {
        private readonly IMovementService _movementService;
        private readonly IMovementCartService _movementCartService;

        public MovementController(IMovementService movementService, IMovementCartService movementCartService)
        {
            _movementService = movementService;
            _movementCartService = movementCartService;
        }

        //get movement history
        [HttpGet("/api/movement/list")]
        public async Task<IActionResult> GetMovementList()
        {
            return Ok(await _movementService.GetMovementList());
        }

        //add movement
        [HttpPost("/api/movement/add")]
        public async Task<IActionResult> AddMovement([FromBody] Movement movement)
        {
            return Ok(await _movementService.AddMovement(movement));
        }

        // cart
        [HttpGet("/api/movement/cart/list")]
        public async Task<IActionResult> GetMovementCart()
        {
            return Ok(await _movementCartService.GetMovementCart());
        }

        // add to cart
        [HttpPost("/api/movement/cart/add")]
        public async Task<IActionResult> AddToMovementCart([FromBody] MovementCart movementCart)
        {
            return Ok(await _movementCartService.AddToMovementCart(movementCart));
        }

        //delete item from cart
        [HttpDelete("/api/movement/cart/remove/{id}")]
        public async Task<IActionResult> RemoveItemFromCart([FromRoute] string id)
        {
            return Ok(await _movementCartService.RemoveItemFromCart(id));
        }

    }
}