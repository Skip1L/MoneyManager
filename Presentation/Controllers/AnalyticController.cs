using DTOs.AnalyticDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AnalyticController(IAnalyticService analyticService) : ControllerBase
    {
        private readonly IAnalyticService _analyticsService = analyticService;

        [HttpPost()]
        public async Task<IActionResult> GetAnalytics([FromBody] AnalyticFilter filter, CancellationToken cancellationToken)
        {
            var result = await _analyticsService.GetAnalyticsByFilter(filter, cancellationToken);
            return Ok(result);
        }
    }
}
