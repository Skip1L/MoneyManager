﻿using System.Diagnostics;
using System.Net.Http.Json;
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
using Services.Jobs.Constants;
using Services.RepositoryInterfaces;

namespace Services.Jobs
{
    public class WeeklyAnalyticJob(ILogger<WeeklyAnalyticJob> logger, IServiceScopeFactory serviceScopeFactory, IHttpClientFactory httpClientFactory, IMapper mapper)
    {
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<WeeklyAnalyticJob> _logger = logger;
        private const string sendWeeklyReports = "/send-weekly-reports";

        public async Task Execute()
        {
            var dateRange = GetLastWeekDateRangeFilter();

            using var scope = _serviceScopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var users = await userManager.GetUsersInRoleAsync(Roles.DefaultUser);

            _logger.LogInformation("Weekly Analytic Job Started\n" +
                $"From: {dateRange.From}; To: {dateRange.To}\n" +
                $"For users: {string.Join(',', users.Select(u => u.Id))}");

            var stopWatch = Stopwatch.StartNew();
            stopWatch.Start();

            await Parallel.ForEachAsync(
                users,
                new ParallelOptions { 
                    MaxDegreeOfParallelism = 10,
                }, 
                async (user, cancellationToken) =>
                {
                    await ProcessAndSendEmailAsync(user, dateRange, cancellationToken);
                });

            stopWatch.Stop();
            _logger.LogInformation($"Weekly Analytic Job Finished. Time Elapsed: {stopWatch.Elapsed}");
        }

        private async Task ProcessAndSendEmailAsync(User user, DateRangeFilter dateRange, CancellationToken cancellationToken)
        {
            var emailReport = new AnalyticEmailRequestDTO
            {
                RecipientName = $"{user.FirstName} {user.LastName}",
                ToEmail = user.Email,
                DateRange = dateRange,
                Incomes = await GetIncomeReportsAsync(user, dateRange, cancellationToken),
                Expenses = await GetExpenseReportsAsync(user, dateRange, cancellationToken),
                Budgets = await GetBudgetReportsAsync(user, dateRange, cancellationToken),
                TransactionsSummary = await GetTotalTransactionsReportsAsync(user, dateRange, cancellationToken)
            };

            await SendEmailReportAsync(emailReport, cancellationToken);
        }

        private async Task<List<CategoryReportDTO>> GetIncomeReportsAsync(User user, DateRangeFilter dateRange, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var incomeRepository = scope.ServiceProvider.GetRequiredService<IIncomeRepository>();

            var filter = new AnalyticFilter
            {
                UserId = user.Id,
                CategoryType = CategoryType.Income,
                DateRangeFilter = dateRange
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

        private async Task<TransactionsSummaryDTO> GetTotalTransactionsReportsAsync(User user, DateRangeFilter dateRange, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var incomeRepository = scope.ServiceProvider.GetRequiredService<IBudgetRepository>();

            var filter = new TransactionFilter
            {
                UserId = user.Id,
                DateRange = dateRange
            };

            return await incomeRepository.GetTransactionsSummaryByDateRangeAsync(filter, cancellationToken);
        }

        private async Task<List<CategoryReportDTO>> GetExpenseReportsAsync(User user, DateRangeFilter dateRange, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var expenseRepository = scope.ServiceProvider.GetRequiredService<IExpenseRepository>();

            var filter = new AnalyticFilter
            {
                UserId = user.Id,
                CategoryType = CategoryType.Expense,
                DateRangeFilter = dateRange
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

        private async Task<List<BudgetReportDTO>> GetBudgetReportsAsync(User user, DateRangeFilter dateRange, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var budgetRepository = scope.ServiceProvider.GetRequiredService<IBudgetRepository>();

            var filter = new AnalyticFilter
            {
                UserId = user.Id,
                CategoryType = null,
                DateRangeFilter = dateRange
            };
         
            var analytics = await budgetRepository.GetTotalTransactionsByBudgetAsync(filter, cancellationToken);

            return _mapper.Map<List<BudgetReportDTO>>(analytics);
        }

        private async Task SendEmailReportAsync(AnalyticEmailRequestDTO emailReport, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient(HttpClientNames.NotificationServiceName);

            var response = await client.PostAsJsonAsync(sendWeeklyReports, emailReport, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to send email report for {emailReport.RecipientName}. StatusCode: {response.StatusCode}");
            }
        }

        private static double CalculatePercentage(double amount, double total)
        {
            return total > 0 ? (amount / total) * 100 : 0;
        }

        private static DateRangeFilter GetLastWeekDateRangeFilter()
        {
            return new DateRangeFilter
            {
                From = DateTime.Today.AddDays(-7),
                To = DateTime.Today
            };
        }
    }
}
