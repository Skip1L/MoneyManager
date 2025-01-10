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
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDTO categoryDTO, CancellationToken cancellationToken)
        {
            await _categoryService.CreateCategoryAsync(categoryDTO, cancellationToken);
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDTO categoryDTO, CancellationToken cancellationToken)
        {
            await _categoryService.UpdateCategoryAsync(categoryDTO, cancellationToken);
            return Ok();
        }

        [HttpDelete("{categoryId}")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid categoryId, CancellationToken cancellationToken)
        {
            await _categoryService.DeleteCategoryAsync(categoryId, cancellationToken);
            return Ok();
        }

        [HttpPost("filter")]
        [Authorize]
        public async Task<IActionResult> FilterCategory([FromBody] PagginationDTO paginationDto, CancellationToken cancellationToken)
        {
            var result = await _categoryService.FilterCategoryAsync(paginationDto, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{categoryId}")]
        [Authorize]
        public async Task<IActionResult> GetCategoryById([FromRoute] Guid categoryId, CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetCategoryByIdAsync(categoryId, cancellationToken);
            return Ok(result);
        }
    }
}
