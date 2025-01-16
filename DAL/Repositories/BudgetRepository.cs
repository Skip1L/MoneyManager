using Domain.Entities;
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
    }
}
