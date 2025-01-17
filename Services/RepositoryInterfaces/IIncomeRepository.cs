using Domain.Entities;

namespace Services.RepositoryInterfaces
{
    public interface IIncomeRepository : IGenericRepository<Income>
    {
        Task<Income> GetIncomeWithBudgetAndCategoryAsync(Guid transactionId, CancellationToken cancellationToken = default);
    }
}
