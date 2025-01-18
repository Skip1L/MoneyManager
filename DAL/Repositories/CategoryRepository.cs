using Domain.Entities;
using DTOs.AnalyticDTOs;
using Microsoft.EntityFrameworkCore;
using Services.RepositoryInterfaces;

namespace DAL.Repositories
{
    public class CategoryRepository(ApplicationContext applicationContext) : GenericRepository<Category>(applicationContext), ICategoryRepository
    {
        public async Task<List<AnalyticDTO>> GetIncomeAnalyticsByFilter(AnalyticFilter filter, CancellationToken cancellationToken)
        {
            if (filter is null)
            {
                throw new ArgumentException("Filter is empty");
            }

            var query = _repositoryContext.Incomes
                .AsNoTracking()
                .Where(i => i.Budget.UserId == filter.UserId);

            if (filter.DataFilter.DateRangeFilter?.From != null)
            {
                query = query.Where(i => i.IncomeDate >= filter.DataFilter.DateRangeFilter.From);
            }
            if (filter.DataFilter.DateRangeFilter?.To != null)
            {
                query = query.Where(i => i.IncomeDate <= filter.DataFilter.DateRangeFilter.To);
            }

            var groupedIncomes = query
                .GroupBy(i => new { i.CategoryId, CategoryName = i.Category.Name })
                .Select(group => new AnalyticDTO
                {
                    Id = group.Key.CategoryId,
                    Name = group.Key.CategoryName,
                    TotalIncome = group.Sum(i => i.Amount)
                });

            var incomeAnalytics = await groupedIncomes.ToListAsync(cancellationToken);

            return incomeAnalytics;
        }

        public async Task<List<AnalyticDTO>> GetExpenseAnalyticsByFilter(AnalyticFilter filter, CancellationToken cancellationToken)
        {
            if (filter is null)
            {
                throw new ArgumentException("Filter is empty");
            }

            var query = _repositoryContext.Expenses
                .AsNoTracking()
                .Where(e => e.Budget.UserId == filter.UserId);

            if (filter.DataFilter.DateRangeFilter?.From != null)
            {
                query = query.Where(e => e.ExpenseDate >= filter.DataFilter.DateRangeFilter.From);
            }
            if (filter.DataFilter.DateRangeFilter?.To != null)
            {
                query = query.Where(e => e.ExpenseDate <= filter.DataFilter.DateRangeFilter.To);
            }

            var groupedExpenses = query
                .GroupBy(e => new { e.CategoryId, CategoryName = e.Category.Name })
                .Select(group => new AnalyticDTO
                {
                    Id = group.Key.CategoryId,
                    Name = group.Key.CategoryName,
                    TotalExpense = -group.Sum(e => e.Amount) // Negative values for expenses
                });

            var expenseAnalytics = await groupedExpenses.ToListAsync(cancellationToken);

            return expenseAnalytics;
        }
    }
}
