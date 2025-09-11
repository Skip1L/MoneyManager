using System.Net.Http.Json;
using AutoMapper;
using DTOs.NotidicationDTOs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.Jobs.Constants;

namespace Services.Jobs
{
    public class HttpWeeklyAnalyticJob(ILogger<HttpWeeklyAnalyticJob> logger,
                                       IServiceScopeFactory serviceScopeFactory,
                                       IHttpClientFactory httpClientFactory,
                                       IMapper mapper) : BaseWeeklyAnalyticJob(logger, serviceScopeFactory, mapper)
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private const string sendWeeklyReports = "/send-weekly-reports";

        protected override async Task SendReportAsync(AnalyticEmailRequestDTO emailReport, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient(HttpClientNames.NotificationServiceName);
            var response = await client.PostAsJsonAsync(sendWeeklyReports, emailReport, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Failed to send email report for {emailReport.RecipientName}. StatusCode: {response.StatusCode}");
            }
        }
    }
}
