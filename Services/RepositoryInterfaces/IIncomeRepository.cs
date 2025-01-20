using Domain.Entities;
using DTOs.AnalyticDTOs;

namespace Services.RepositoryInterfaces
{
    public interface IIncomeRepository : IGenericRepository<Income>
    {
        Task<Income> GetIncomeWithBudgetAndCategoryAsync(Guid transactionId, CancellationToken cancellationToken = default);
        Task<List<AnalyticDTO>> GetTotalIncomeByCategoriesAsync(AnalyticFilter filter, CancellationToken cancellationToken);
    }
}
