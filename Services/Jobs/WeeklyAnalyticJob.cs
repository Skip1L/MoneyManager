using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using DTOs.AnalyticDTOs;
using DTOs.CommonDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.Interfaces;

namespace Services.Jobs
{
    public class WeeklyAnalyticJob(ILogger<WeeklyAnalyticJob> logger, IServiceScopeFactory serviceScopeFactory)
    {
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
        private readonly ILogger<WeeklyAnalyticJob> _logger = logger;

        public async Task Execute(CancellationToken cancellationToken)
        {
            var categoryTypes = Enum.GetValues(typeof(CategoryType))
                            .Cast<CategoryType?>()
                            .Concat([null])
                            .ToArray();

            using var scope = _serviceScopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var users = await userManager.GetUsersInRoleAsync(Roles.DefaultUser);

            var tasks = new List<Task>();

            foreach (var user in users)
            {
                foreach (var categoryType in categoryTypes)
                {
                    tasks.Add(ProcessAnalyticsAsync(user, categoryType, cancellationToken));
                }
            }

            await Task.WhenAll(tasks);
        }

        private async Task ProcessAnalyticsAsync(User user, CategoryType? categoryType, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var analyticService = scope.ServiceProvider.GetRequiredService<IAnalyticService>();

            var filter = new AnalyticFilter
            {
                UserId = user.Id,
                CategoryType = categoryType,
                PaginationFilter = new PaginationFilter { PageNumber = 1, PageSize = 10 },
                DateRangeFilter = new DateRangeFilter { From = DateTime.UtcNow.AddDays(-7), To = DateTime.UtcNow }
            };

            var analytics = await analyticService.GetAnalyticsByFilter(filter, cancellationToken);

            foreach (var analytic in analytics)
            {
                _logger.LogInformation(
                    $"UserName: {user.UserName}\n" +
                    $"CategoryType: {categoryType}\n" +
                    $"AnalyticId: {analytic.Id}\n" +
                    $"AnalyticName: {analytic.Name}\n" +
                    $"AnalyticIncome: {analytic.TotalIncome}\n" +
                    $"AnalyticExpense: {analytic.TotalExpense}\n");
            }
        }
    }
}
