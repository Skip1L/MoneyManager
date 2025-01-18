using Domain.Entities;
using DTOs.AnalyticDTOs;

namespace Services.RepositoryInterfaces
{
    public interface IExpenseRepository : IGenericRepository<Expense>
    {
        Task<Expense> GetExpenseWithBudgetAndCategoryAsync(Guid transactionId, CancellationToken cancellationToken = default);
        Task<List<AnalyticDTO>> GetTotalExpenseByCategoriesAsync(AnalyticFilter filter, CancellationToken cancellationToken);
    }
}
