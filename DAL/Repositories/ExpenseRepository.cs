using Domain.Entities;
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
    }
}
