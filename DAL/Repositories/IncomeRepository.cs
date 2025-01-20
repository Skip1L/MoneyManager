using Domain.Entities;
using DTOs.AnalyticDTOs;
using Microsoft.EntityFrameworkCore;
using Services.RepositoryInterfaces;

namespace DAL.Repositories
{
    public class IncomeRepository(ApplicationContext repositoryContext) : GenericRepository<Income>(repositoryContext), IIncomeRepository
    {
        public async Task<Income> GetIncomeWithBudgetAndCategoryAsync(Guid transactionId, CancellationToken cancellationToken = default)
        {
            return await _repositoryContext
                .Incomes
                .AsNoTracking()
                .Include(b => b.Budget)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == transactionId, cancellationToken);
        }

        public async Task<List<AnalyticDTO>> GetTotalIncomeByCategoriesAsync(AnalyticFilter filter, CancellationToken cancellationToken)
        {
            if (filter is null)
            {
                throw new ArgumentException("Filter is empty");
            }

            var query = _repositoryContext
                .Incomes
                .AsNoTracking()
                .Where(i => i.Budget.UserId == filter.UserId);

            if (filter.DateRangeFilter?.From != null)
            {
                query = query.Where(i => i.IncomeDate >= filter.DateRangeFilter.From);
            }

            if (filter.DateRangeFilter?.To != null)
            {
                query = query.Where(i => i.IncomeDate <= filter.DateRangeFilter.To);
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

    }
}
