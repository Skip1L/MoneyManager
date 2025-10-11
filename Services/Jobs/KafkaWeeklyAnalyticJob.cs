using AutoMapper;
using DTOs.Exchanges;
using DTOs.NotidicationDTOs;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Services.Jobs
{
    public class KafkaWeeklyAnalyticJob(ILogger<KafkaWeeklyAnalyticJob> logger,
                                           IServiceScopeFactory serviceScopeFactory,
                                           ITopicProducer<string, AnalyticEmailCreated> producer,
                                           IMapper mapper) : BaseWeeklyAnalyticJob(logger, serviceScopeFactory, mapper)
    {
        private readonly ITopicProducer<string, AnalyticEmailCreated> _producer = producer;

        protected override async Task SendReportAsync(AnalyticEmailRequestDTO emailReport, CancellationToken cancellationToken)
        {
            var randomKey = Guid.NewGuid().ToString("N");

            await _producer.Produce(
                key: randomKey,
                values: new
                {
                    Id = Guid.NewGuid(),
                    Budgets = emailReport.Budgets,
                    DateRange = emailReport.DateRange,
                    Expenses = emailReport.Expenses,
                    Incomes = emailReport.Incomes,
                    RecipientName = emailReport.RecipientName,
                    ToEmail = emailReport.ToEmail,
                    TransactionsSummary = emailReport.TransactionsSummary
                },
                cancellationToken: cancellationToken);

            logger.LogInformation("Sent Report");
        }
    }
}
