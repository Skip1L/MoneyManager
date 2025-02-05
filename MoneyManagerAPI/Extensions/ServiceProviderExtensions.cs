using Domain.Constants;
using Hangfire;
using Services.Jobs;

namespace MoneyManagerAPI.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static void AddRecurringJobs(this IServiceProvider serviceProvider)
        {
            RecurringJob.AddOrUpdate<GrpcWeeklyAnalyticJob>(
                nameof(GrpcWeeklyAnalyticJob),
                job => job.Execute(),
                Cron.Weekly(DayOfWeek.Monday, 9),
                new RecurringJobOptions()
            );
        }
    }
}
