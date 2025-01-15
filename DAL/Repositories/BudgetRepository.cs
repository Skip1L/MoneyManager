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

        public async Task<Income> GetIncomeWithBudgetAsync(Guid transactionId, CancellationToken cancellationToken = default)
        {
            return await _repositoryContext
                .Incomes
                .AsNoTracking()
                .Include(b => b.Budget).Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == transactionId, cancellationToken);
        }

        public async Task<Expense> GetExpenseWithBudgetAsync(Guid transactionId, CancellationToken cancellationToken = default)
        {
            return await _repositoryContext
                .Expenses
                .AsNoTracking()
                .Include(b => b.Budget).Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == transactionId, cancellationToken);
        }

        public async Task<List<TransactionDTO>> GetTransactionsByTransactionFilter(TransactionFilter filter, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filter?.UserId.ToString()))
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

            if (filter.DateRangePaginationDTO?.From != null)
            {
                expensesQuery = expensesQuery.Where(e => e.Date >= filter.DateRangePaginationDTO.From);
                incomesQuery = incomesQuery.Where(e => e.Date >= filter.DateRangePaginationDTO.From);
            }

            if (filter.DateRangePaginationDTO?.To != null)
            {
                expensesQuery = expensesQuery.Where(e => e.Date <= filter.DateRangePaginationDTO.To);
                incomesQuery = incomesQuery.Where(e => e.Date <= filter.DateRangePaginationDTO.To);
            }

            var combinedQuery = incomesQuery
                .Union(expensesQuery)
                .OrderByDescending(e => e.Date);

            if (filter.DateRangePaginationDTO?.Pagination != null)
            {
                combinedQuery = combinedQuery
                    .Skip(filter.DateRangePaginationDTO.Pagination.PageNumber * filter.DateRangePaginationDTO.Pagination.PageSize)
                    .Take(filter.DateRangePaginationDTO.Pagination.PageSize)
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

        public async Task<TransactionsSummaryDTO> GetTransactionsSummaryByDateRange(TransactionSummaryFilter filter, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filter?.UserId.ToString()))
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

            var totalIncomesTask = incomesQuery.SumAsync(_ => _.Amount);
            var totalExpensesTask = expensesQuery.SumAsync(_ => _.Amount);

            var result = await Task.WhenAll(totalIncomesTask, totalExpensesTask);

            return new TransactionsSummaryDTO
            {
                TotalIncomes = result[0],
                TotalExpenses = result[1]
            };
        }
    }
}
