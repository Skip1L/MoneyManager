using Domain.Entities;
using Domain.Enums;
using DTOs.TransactionDTOs;
using Microsoft.EntityFrameworkCore;
using Services.RepositoryInterfaces;

namespace DAL.Repositories
{
    public class BudgetRepository(ApplicationContext repositoryContext) : GenericRepository<Budget>(repositoryContext), IBudgetRepository
    {
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

            var budgetIds = _repositoryContext
                .Budgets
                .Where(budget => budget.UserId == filter.UserId)
                .Select(budget => budget.Id);

            var expensesQuery = _repositoryContext
                .Expenses
                .Where(e => budgetIds.Contains(e.BudgetId))
                .Select(e => new
                {
                    Date = e.ExpenseDate,
                    e.Amount
                });

            var incomesQuery = _repositoryContext
                .Incomes
                .Where(i => budgetIds.Contains(i.BudgetId))
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

            await Task.WhenAll(totalIncomesTask, totalExpensesTask);

            return new TransactionsSummaryDTO
            {
                TotalIncomes = totalIncomesTask.Result,
                TotalExpenses = totalExpensesTask.Result,
                NetBalance = totalIncomesTask.Result - totalExpensesTask.Result
            };
        }
    }
}
