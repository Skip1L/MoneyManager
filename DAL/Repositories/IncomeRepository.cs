using Domain.Entities;
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
    }
}
