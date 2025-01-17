using Domain.Entities;

namespace Services.RepositoryInterfaces
{
    public interface IExpenseRepository : IGenericRepository<Expense>
    {
        Task<Expense> GetExpenseWithBudgetAndCategoryAsync(Guid transactionId, CancellationToken cancellationToken = default);
    }
}
