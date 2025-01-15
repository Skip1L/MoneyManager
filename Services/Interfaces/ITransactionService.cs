using Domain.Enums;
using DTOs.TransactionDTOs;

namespace Services.Interfaces
{
    public interface ITransactionService
    {
        Task CreateTransactionAsync(CreateTransactionDTO transactionDTO, CancellationToken cancellationToken);
        Task DeleteTransactionAsync(Guid transactionId, CategoryType categoryType, string userName, CancellationToken cancellationToken);
        Task<List<TransactionDTO>> GetByFilterAsync(TransactionFilter filter, CancellationToken cancellationToken);
        Task<TransactionDTO> GetTransactionByIdAsync(Guid transactionId, CategoryType categoryType, Guid userId, CancellationToken cancellationToken);
        Task UpdateTransactionAsync(UpdateTransactionDTO transactionDTO, CancellationToken cancellationToken);
        Task<TransactionsSummaryDTO> GetTransactionsSummary(TransactionSummaryFilter filter, CancellationToken cancellationToken);
    }
}
