using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Services.Jobs;

namespace Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class TestController(IServiceScopeFactory serviceScopeFactory) : ControllerBase
    {
        [HttpPost("send-analytic/{transportType}")]
        public async Task<IActionResult> SendAnalytic([FromRoute] int transportType)
        {
            using var serviceScope = serviceScopeFactory.CreateScope();

            if (transportType == 0)
            {
                await serviceScope.ServiceProvider.GetRequiredService<GrpcWeeklyAnalyticJob>().Execute();
            }

            else if (transportType == 1)
            {
                await serviceScope.ServiceProvider.GetRequiredService<HttpWeeklyAnalyticJob>().Execute();
            }

            else if (transportType == 2)
            {
                await serviceScope.ServiceProvider.GetRequiredService<KafkaWeeklyAnalyticJob>().Execute();
            }

            else if (transportType == 3)
            {
                await serviceScope.ServiceProvider.GetRequiredService<RabbitMqWeeklyAnalyticJob>().Execute();
            }
            else
            {
                return BadRequest();
            }

            return NoContent();
        }
    }
}
