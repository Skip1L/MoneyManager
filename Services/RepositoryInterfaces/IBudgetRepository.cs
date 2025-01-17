using Domain.Entities;
using DTOs.TransactionDTOs;

namespace Services.RepositoryInterfaces
{
    public interface IBudgetRepository : IGenericRepository<Budget>
    {
        Task<Budget> GetBudgetWithUserAsync(Guid budgetId, CancellationToken cancellationToken = default);
        Task<List<TransactionDTO>> GetTransactionsByTransactionFilter(TransactionFilter filter, CancellationToken cancellationToken = default);
        Task<TransactionsSummaryDTO> GetTransactionsSummaryByDateRange(TransactionFilter filter, CancellationToken cancellationToken = default);
    }
}
