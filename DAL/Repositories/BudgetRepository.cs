using Domain.Entities;
using Domain.Enums;
using DTOs.AnalyticDTOs;
using DTOs.TransactionDTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.RepositoryInterfaces;

namespace DAL.Repositories
{
    public class BudgetRepository(ApplicationContext repositoryContext, IServiceProvider serviceProvider) : GenericRepository<Budget>(repositoryContext), IBudgetRepository
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public async Task<Budget> GetBudgetWithUserAsync(Guid budgetId, CancellationToken cancellationToken = default)
        {
            return await _repositoryContext
                .Budgets
                .AsNoTracking()
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == budgetId, cancellationToken);
        }

        public async Task<List<TransactionDTO>> GetTransactionsByTransactionFilterAsync(TransactionFilter filter, CancellationToken cancellationToken = default)
        {
            if (filter is null)
            {
                throw new ArgumentException("Filter is empty");
            }

            var budgetIds = _repositoryContext
                .Budgets
                .Where(budget => budget.UserId == filter.UserId)
                .Select(budget => budget.Id);

            var expensesQuery = _repositoryContext
                .Expenses
                .Where(e => budgetIds.Contains(e.BudgetId))
                .Select(e => new
                {
                    e.Id,
                    Date = e.ExpenseDate,
                    e.Amount,
                    Type = CategoryType.Expense,
                    e.Description,
                    BudgetName = e.Budget.Name,
                    CategoryName = e.Category.Name
                });

            var incomesQuery = _repositoryContext
                .Incomes
                .Where(i => budgetIds.Contains(i.BudgetId))
                .Select(e => new
                {
                    e.Id,
                    Date = e.IncomeDate,
                    e.Amount,
                    Type = CategoryType.Income,
                    e.Description,
                    BudgetName = e.Budget.Name,
                    CategoryName = e.Category.Name
                });

            if (filter.DateRange?.From != null)
            {
                expensesQuery = expensesQuery.Where(e => e.Date >= filter.DateRange.From);
                incomesQuery = incomesQuery.Where(e => e.Date >= filter.DateRange.From);
            }

            if (filter.DateRange?.To != null)
            {
                expensesQuery = expensesQuery.Where(e => e.Date <= filter.DateRange.To);
                incomesQuery = incomesQuery.Where(e => e.Date <= filter.DateRange.To);
            }

            var combinedQuery = incomesQuery
                .Union(expensesQuery)
                .OrderByDescending(e => e.Date);

            if (filter.Pagination != null)
            {
                combinedQuery = combinedQuery
                    .Skip(filter.Pagination.PageNumber * filter.Pagination.PageSize)
                    .Take(filter.Pagination.PageSize)
                    .OrderByDescending(e => e.Date);
            }

            var combinedResult = await combinedQuery
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var result = combinedResult.Select(a => new TransactionDTO
            {
                Id = a.Id,
                Amount = a.Amount,
                CategoryType = a.Type,
                Description = a.Description,
                TransactionDate = a.Date,
                BudgetName = a.BudgetName,
                CategoryName = a.CategoryName
            }).ToList();

            return result;
        }

        public async Task<TransactionsSummaryDTO> GetTransactionsSummaryByDateRangeAsync(TransactionFilter filter, CancellationToken cancellationToken = default)
        {
            if (filter is null)
            {
                throw new ArgumentException("Filter is empty");
            }

            using var scopeExpense = _serviceProvider.CreateScope();
            using var scopeIncome = _serviceProvider.CreateScope();
            var expensesContext = scopeExpense.ServiceProvider.GetRequiredService<ApplicationContext>();
            var incomesContext = scopeIncome.ServiceProvider.GetRequiredService<ApplicationContext>();

            var budgetIdsExpense = expensesContext
                .Budgets
                .AsNoTracking()
                .Where(budget => budget.UserId == filter.UserId)
                .Select(budget => budget.Id);

            var budgetIdsIncome = incomesContext
                .Budgets
                .AsNoTracking()
                .Where(budget => budget.UserId == filter.UserId)
                .Select(budget => budget.Id);

            var expensesQuery = expensesContext
                .Expenses
                .AsNoTracking()
                .Where(e => budgetIdsExpense.Contains(e.BudgetId))
                .Select(e => new
                {
                    Date = e.ExpenseDate,
                    e.Amount
                });

            var incomesQuery = incomesContext
                .Incomes
                .AsNoTracking()
                .Where(i => budgetIdsIncome.Contains(i.BudgetId))
                .Select(e => new
                {
                    Date = e.IncomeDate,
                    e.Amount,
                });

            if (filter.DateRange?.From != null)
            {
                expensesQuery = expensesQuery.Where(e => e.Date >= filter.DateRange.From);
                incomesQuery = incomesQuery.Where(e => e.Date >= filter.DateRange.From);
            }

            if (filter.DateRange?.To != null)
            {
                expensesQuery = expensesQuery.Where(e => e.Date <= filter.DateRange.To);
                incomesQuery = incomesQuery.Where(e => e.Date <= filter.DateRange.To);
            }

            var totalIncomesTask = incomesQuery.SumAsync(i => i.Amount, cancellationToken);
            var totalExpensesTask = expensesQuery.SumAsync(e => e.Amount, cancellationToken);

            return new TransactionsSummaryDTO
            {
                TotalIncomes = await totalIncomesTask,
                TotalExpenses = await totalExpensesTask,
                NetBalance = await totalIncomesTask - await totalExpensesTask
            };
        }

        public async Task<List<AnalyticDTO>> GetBudgetsAnalyticByFilter(AnalyticFilter filter, CancellationToken cancellationToken)
        {
            if (filter is null)
            {
                throw new ArgumentException("Filter is empty");
            }

            var query = _repositoryContext.Budgets
                .AsNoTracking()
                .Where(budget => budget.UserId == filter.UserId)
                .Select(budget => new
                {
                    BudgetId = budget.Id,
                    BudgetName = budget.Name,
                    Incomes = budget.Incomes.Select(i => new
                    {
                        i.IncomeDate,
                        i.Amount
                    }),
                    Expenses = budget.Expenses.Select(i => new
                    {
                        i.ExpenseDate,
                        i.Amount
                    })
                });

            if (filter.DataFilter.DateRangeFilter?.From != null)
            {
                query = query.Select(budgetData => new
                {
                    budgetData.BudgetId,
                    budgetData.BudgetName,
                    Incomes = budgetData.Incomes.Where(i => i.IncomeDate >= filter.DataFilter.DateRangeFilter.From),
                    Expenses = budgetData.Expenses.Where(e => e.ExpenseDate >= filter.DataFilter.DateRangeFilter.From)
                });
            }

            if (filter.DataFilter.DateRangeFilter?.To != null)
            {
                query = query.Select(budgetData => new
                {
                    budgetData.BudgetId,
                    budgetData.BudgetName,
                    Incomes = budgetData.Incomes.Where(i => i.IncomeDate <= filter.DataFilter.DateRangeFilter.To),
                    Expenses = budgetData.Expenses.Where(e => e.ExpenseDate <= filter.DataFilter.DateRangeFilter.To)
                });
            }

            var budgetSummary = await query
                .Select(budgetData => new AnalyticDTO
                {
                    Id = budgetData.BudgetId,
                    Name = budgetData.BudgetName,
                    TotalIncome = budgetData.Incomes.Sum(i => i.Amount),
                    TotalExpense = budgetData.Expenses.Sum(e => e.Amount)
                })
                .ToListAsync(cancellationToken);

            return budgetSummary;
        }
    }
}
