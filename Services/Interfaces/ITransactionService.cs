using Domain.Enums;
using DTOs.TransactionDTOs;

namespace Services.Interfaces
{
    public interface ITransactionService
    {
        Task CreateTransactionAsync(CreateTransactionDTO transactionDTO, CancellationToken cancellationToken);
        Task DeleteTransactionAsync(Guid transactionId, Guid userId, CancellationToken cancellationToken);
        Task<List<TransactionDTO>> GetByFilterAsync(TransactionFilter filter, CancellationToken cancellationToken);
        Task<TransactionDTO> GetTransactionByIdAsync(Guid transactionId, Guid userId, CancellationToken cancellationToken);
        Task UpdateTransactionAsync(UpdateTransactionDTO transactionDTO, Guid userId, CancellationToken cancellationToken);
        Task<TransactionsSummaryDTO> GetTransactionsSummaryAsync(TransactionFilter filter, CancellationToken cancellationToken);
    }
}
