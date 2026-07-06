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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("/api/category/list")]
        public async Task<IActionResult> GetCategoryList()
        {
            return Ok(await _categoryService.GetCategoryList());
        }

        [HttpPost("/api/category/add")]
        public async Task<IActionResult> AddCategory([FromBody] Model.Category category){
            return Ok(await _categoryService.AddCategory(category));
        }
    }
}