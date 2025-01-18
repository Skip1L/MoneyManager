using Domain.Entities;
using DTOs.AnalyticDTOs;
using DTOs.TransactionDTOs;

namespace Services.RepositoryInterfaces
{
    public interface IBudgetRepository : IGenericRepository<Budget>
    {
        Task<List<AnalyticDTO>> GetBudgetsAnalyticByFilter(AnalyticFilter filter, CancellationToken cancellationToken);
        Task<Budget> GetBudgetWithUserAsync(Guid budgetId, CancellationToken cancellationToken = default);
        Task<List<TransactionDTO>> GetTransactionsByTransactionFilterAsync(TransactionFilter filter, CancellationToken cancellationToken = default);
        Task<TransactionsSummaryDTO> GetTransactionsSummaryByDateRangeAsync(TransactionFilter filter, CancellationToken cancellationToken = default);
    }
}
