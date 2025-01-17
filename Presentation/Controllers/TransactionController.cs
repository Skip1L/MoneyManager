using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using DTOs.TransactionDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class TransactionController(ITransactionService transactionService, UserManager<User> userManager) : ControllerBase
    {
        private readonly ITransactionService _transactionService = transactionService;
        private readonly UserManager<User> _userManager = userManager;

        [HttpPost]
        [Authorize(Roles = Roles.DefaultUser)]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDTO transactionDTO, CancellationToken cancellationToken)
        {
            await _transactionService.CreateTransactionAsync(transactionDTO, cancellationToken);
            return Ok();
        }

        [HttpPost("filter")]
        [Authorize(Roles = Roles.DefaultUser)]
        public async Task<ActionResult<List<TransactionDTO>>> GetTransactionsByFilter([FromBody] TransactionFilter filter, CancellationToken cancellationToken)
        {
            var transactions = await _transactionService.GetByFilterAsync(filter, cancellationToken);
            return Ok(transactions);
        }

        [HttpPut]
        [Authorize(Roles = Roles.DefaultUser)]
        public async Task<IActionResult> UpdateTransaction([FromBody] UpdateTransactionDTO transactionDTO, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity!.Name!);
            await _transactionService.UpdateTransactionAsync(transactionDTO, user.Id, cancellationToken);
            return Ok();
        }

        [HttpDelete("{transactionId}")]
        [Authorize(Roles = Roles.DefaultUser)]
        public async Task<IActionResult> DeleteTransaction([FromRoute] Guid transactionId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity!.Name!);
            await _transactionService.DeleteTransactionAsync(transactionId, user.Id, cancellationToken);
            return Ok();
        }

        [HttpGet("{transactionId}")]
        [Authorize(Roles = Roles.DefaultUser)]
        public async Task<IActionResult> GetTransactionById([FromRoute] Guid transactionId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(HttpContext.User.Identity!.Name!);
            var result = await _transactionService.GetTransactionByIdAsync(transactionId, user.Id, cancellationToken);
            return Ok(result);
        }

        [HttpPost("summary")]
        [Authorize(Roles = Roles.DefaultUser)]
        public async Task<ActionResult<TransactionsSummaryDTO>> GetTransactionsSummary([FromBody] TransactionFilter filter, CancellationToken cancellationToken)
        {
            var transactions = await _transactionService.GetTransactionsSummary(filter, cancellationToken);
            return Ok(transactions);
        }
    }
}
