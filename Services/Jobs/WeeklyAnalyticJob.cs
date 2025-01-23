using System.Net.Http.Json;
using System.Text.Json;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using DTOs.AnalyticDTOs;
using DTOs.CommonDTOs;
using DTOs.NotidicationDTOs;
using DTOs.TransactionDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.RepositoryInterfaces;

namespace Services.Jobs
{
    public class WeeklyAnalyticJob(ILogger<WeeklyAnalyticJob> logger, IServiceScopeFactory serviceScopeFactory, HttpClient httpClient, IMapper mapper)
    {
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
        private readonly HttpClient _httpClient = httpClient;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<WeeklyAnalyticJob> _logger = logger;

        public async Task Execute(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var users = await userManager.GetUsersInRoleAsync(Roles.DefaultUser);

            var tasks = new List<Task>();

            foreach (var user in users)
            {
                tasks.Add(ProcessAndSendEmailAsync(user, cancellationToken));
            }

            await Task.WhenAll(tasks);
        }

        private async Task ProcessAndSendEmailAsync(User user, CancellationToken cancellationToken)
        {
            var emailReport = new AnalyticEmailRequestDTO
            {
                RecipientName = $"{user.FirstName} {user.LastName}",
                ToEmail = user.Email,
                Incomes = await GetIncomeReportsAsync(user, cancellationToken),
                Expenses = await GetExpenseReportsAsync(user, cancellationToken),
                Budgets = await GetBudgetReportsAsync(user, cancellationToken),
                TransactionsSummary = await GetTotalTransactionsReportsAsync(user, cancellationToken)
            };

            await SendEmailReportAsync(emailReport, cancellationToken);
        }

        private async Task<List<CategoryReportDTO>> GetIncomeReportsAsync(User user, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var incomeRepository = scope.ServiceProvider.GetRequiredService<IIncomeRepository>();

            var filter = new AnalyticFilter
            {
                UserId = user.Id,
                CategoryType = CategoryType.Income,
                PaginationFilter = new PaginationFilter { PageNumber = 1, PageSize = 10 },
                DateRangeFilter = new DateRangeFilter { From = DateTime.MinValue, To = DateTime.UtcNow }
            };

            var analytics = await incomeRepository.GetTotalIncomeByCategoriesAsync(filter, cancellationToken);
            var totalIncome = analytics.Sum(a => a.TotalIncome);

            return _mapper.Map<List<CategoryReportDTO>>(analytics)
                   .Select(a => {
                       a.Percentage = Math.Round(CalculatePercentage(a.Amount, totalIncome), 2);
                       return a;
                   })
                   .ToList();
        }

        private async Task<TransactionsSummaryDTO> GetTotalTransactionsReportsAsync(User user, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var incomeRepository = scope.ServiceProvider.GetRequiredService<IBudgetRepository>();

            var filter = new TransactionFilter
            {
                UserId = user.Id,
                Pagination = new PaginationFilter { PageNumber = 1, PageSize = 10 },
                DateRange = new DateRangeFilter { From = DateTime.MinValue, To = DateTime.UtcNow }
            };

            return await incomeRepository.GetTransactionsSummaryByDateRangeAsync(filter, cancellationToken);
        }

        private async Task<List<CategoryReportDTO>> GetExpenseReportsAsync(User user, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var expenseRepository = scope.ServiceProvider.GetRequiredService<IExpenseRepository>();

            var filter = new AnalyticFilter
            {
                UserId = user.Id,
                CategoryType = CategoryType.Expense,
                PaginationFilter = new PaginationFilter { PageNumber = 1, PageSize = 10 },
                DateRangeFilter = new DateRangeFilter { From = DateTime.MinValue, To = DateTime.UtcNow }
            };

            var analytics = await expenseRepository.GetTotalExpenseByCategoriesAsync(filter, cancellationToken);
            var totalExpense = analytics.Sum(a => a.TotalExpense);

            return _mapper.Map<List<CategoryReportDTO>>(analytics)
                  .Select(a => {
                      a.Percentage = Math.Round(CalculatePercentage(a.Amount, totalExpense), 2);
                      return a;
                  })
                  .ToList();
        }

        private async Task<List<BudgetReportDTO>> GetBudgetReportsAsync(User user, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var budgetRepository = scope.ServiceProvider.GetRequiredService<IBudgetRepository>();

            var filter = new AnalyticFilter
            {
                UserId = user.Id,
                CategoryType = null,
                PaginationFilter = new PaginationFilter { PageNumber = 1, PageSize = 10 },
                DateRangeFilter = new DateRangeFilter { From = DateTime.UtcNow.AddDays(-7), To = DateTime.UtcNow }
            };
         
            var analytics = await budgetRepository.GetTotalTransactionsByBudgetAsync(filter, cancellationToken);

            return _mapper.Map<List<BudgetReportDTO>>(analytics);
        }

        private async Task SendEmailReportAsync(AnalyticEmailRequestDTO emailReport, CancellationToken cancellationToken)
        {
            _logger.LogInformation(JsonSerializer.Serialize(emailReport));

            var response = await _httpClient.PostAsJsonAsync("http://localhost:6002/send-weekly-reports", emailReport, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to send email report for {emailReport.RecipientName}. StatusCode: {response.StatusCode}");
            }
        }

        private double CalculatePercentage(double amount, double total)
        {
            return total > 0 ? (amount / total) * 100 : 0;
        }
    }
}
