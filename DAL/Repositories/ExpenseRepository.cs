using Domain.Entities;
using DTOs.AnalyticDTOs;
using Microsoft.EntityFrameworkCore;
using Services.RepositoryInterfaces;

namespace DAL.Repositories
{
    public class ExpenseRepository(ApplicationContext repositoryContext) : GenericRepository<Expense>(repositoryContext), IExpenseRepository
    {
        public async Task<Expense> GetExpenseWithBudgetAndCategoryAsync(Guid transactionId, CancellationToken cancellationToken = default)
        {
            return await _repositoryContext
                .Expenses
                .AsNoTracking()
                .Include(b => b.Budget)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == transactionId, cancellationToken);
        }

        public async Task<List<AnalyticDTO>> GetTotalExpenseByCategoriesAsync(AnalyticFilter filter, CancellationToken cancellationToken)
        {
            if (filter is null)
            {
                throw new ArgumentException("Filter is empty");
            }

            var query = _repositoryContext
                .Expenses
                .AsNoTracking()
                .Where(e => e.Budget.UserId == filter.UserId);

            if (filter.DateRangeFilter?.From != null)
            {
                query = query.Where(e => e.ExpenseDate >= filter.DateRangeFilter.From);
            }

            if (filter.DateRangeFilter?.To != null)
            {
                query = query.Where(e => e.ExpenseDate <= filter.DateRangeFilter.To);
            }

            var groupedExpenses = query
                .GroupBy(e => new { e.CategoryId, CategoryName = e.Category.Name })
                .Select(group => new AnalyticDTO
                {
                    Id = group.Key.CategoryId,
                    Name = group.Key.CategoryName,
                    TotalExpense = group.Sum(e => e.Amount)
                });

            var expenseAnalytics = await groupedExpenses.ToListAsync(cancellationToken);

            return expenseAnalytics;
        }
    }
}
