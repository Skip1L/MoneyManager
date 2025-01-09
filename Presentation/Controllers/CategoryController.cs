using Domain.Constants;
using DTOs.CategoryDTOs;
using DTOs.CommonDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;

        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDTO categoryDTO)
        {
            await _categoryService.CreateCategoryAsync(categoryDTO);
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDTO categoryDTO)
        {
            await _categoryService.UpdateCategoryAsync(categoryDTO);
            return Ok();
        }

        [HttpDelete("{categoryId}")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid categoryId)
        {
            await _categoryService.DeleteCategoryAsync(categoryId);
            return Ok();
        }

        [HttpPost("filter")]
        [Authorize]
        public async Task<IActionResult> FilterCategory([FromBody] PagginationDTO paginationDto)
        {
            var result = await _categoryService.FilterCategoryAsync(paginationDto);
            return Ok(result);
        }

        [HttpGet("{categoryId}")]
        [Authorize]
        public async Task<IActionResult> GetCategoryById([FromRoute] Guid categoryId)
        {
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);
            return Ok(result);
        }
    }
}
