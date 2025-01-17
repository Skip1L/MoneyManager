using Domain.Constants;
using Domain.Entities;
using DTOs.BudgetDTOs;
using DTOs.CommonDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class BudgetController(IBudgetService budgetService, UserManager<User> userManager) : ControllerBase
    {
        private readonly IBudgetService _budgetService = budgetService;
        private readonly UserManager<User> _userManager = userManager;

        [HttpPost]
        [Authorize(Roles = Roles.DefaultUser)]
        public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetDTO budgetDTO, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity!.Name!);
            await _budgetService.CreateBudgetAsync(budgetDTO, user.Id, cancellationToken);
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = Roles.DefaultUser)]
        public async Task<IActionResult> UpdateBudget([FromBody] UpdateBudgetDTO budgetDTO, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity!.Name!);
            await _budgetService.UpdateBudgetAsync(budgetDTO, user.Id, cancellationToken);
            return Ok();
        }

        [HttpDelete("{budgetId}")]
        [Authorize(Roles = Roles.DefaultUser)]
        public async Task<IActionResult> DeleteBudget([FromRoute] Guid budgetId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity!.Name!);
            await _budgetService.DeleteBudgetAsync(budgetId, user.Id, cancellationToken);
            return Ok();
        }

        [HttpPost("filter")]
        [Authorize(Roles = Roles.DefaultUser)]
        public async Task<IActionResult> FilterBudget([FromBody] PaginationFilter paginationDto, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity!.Name!);
            var result = await _budgetService.FilterBudgetAsync(paginationDto, user.Id, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{budgetId}")]
        [Authorize(Roles = Roles.DefaultUser)]
        public async Task<IActionResult> GetBudgetById([FromRoute] Guid budgetId, CancellationToken cancellationToken)
        {
            var result = await _budgetService.GetBudgetByIdAsync(budgetId, HttpContext.User.Identity.Name, cancellationToken);
            return Ok(result);
        }
    }
}
