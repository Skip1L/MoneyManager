using AutoMapper;
using DTOs.NotidicationDTOs;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.Protos;

namespace Services.Jobs
{
    public class GrpcWeeklyAnalyticJob(GrpcEmail.GrpcEmailClient grpcEmailClient,
                                       ILogger<GrpcWeeklyAnalyticJob> logger,
                                       IServiceScopeFactory serviceScopeFactory,
                                       IMapper mapper) : BaseWeeklyAnalyticJob(logger, serviceScopeFactory, mapper)
    {
        private readonly GrpcEmail.GrpcEmailClient _grpcEmailClient = grpcEmailClient;

        protected override async Task SendReportAsync(AnalyticEmailRequestDTO emailReport, CancellationToken cancellationToken)
        {
            var request = mapper.Map<SendEmailRequest>(emailReport);

            var response = await _grpcEmailClient.SendEmailAsync(request, cancellationToken: cancellationToken);

            if (!response.Success)
            {
                logger.LogError($"Failed to send email report for {emailReport.RecipientName}.");
            }
        }
    }
}
