using AutoMapper;
using DTOs.Exchanges;
using DTOs.NotidicationDTOs;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Services.Jobs
{
    public class RabbitMqWeeklyAnalyticJob(ILogger<HttpWeeklyAnalyticJob> logger,
                                           IServiceScopeFactory serviceScopeFactory,
                                           IPublishEndpoint publishEndpoint,
                                           IMapper mapper) : BaseWeeklyAnalyticJob(logger, serviceScopeFactory, mapper)
    {
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
        protected override async Task SendReportAsync(AnalyticEmailRequestDTO emailReport, CancellationToken cancellationToken)
        {
            await _publishEndpoint.Publish<AnalyticEmailCreated>(new
            {
                Id = new Guid(),
                Budgets = emailReport.Budgets,
                DateRange = emailReport.DateRange,
                Expenses = emailReport.Expenses,
                Incomes = emailReport.Incomes,
                RecipientName = emailReport.RecipientName,
                ToEmail = emailReport.ToEmail,
                TransactionsSummary = emailReport.TransactionsSummary
            }, cancellationToken);

            logger.LogInformation("Sent Report");
        }
    }
}
