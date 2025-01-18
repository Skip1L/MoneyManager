using DTOs.AnalyticDTOs;

namespace Services.RepositoryInterfaces
{
    public interface ICategoryRepository
    {
        Task<List<AnalyticDTO>> GetIncomeAnalyticsByFilter(AnalyticFilter filter, CancellationToken cancellationToken);
        Task<List<AnalyticDTO>> GetExpenseAnalyticsByFilter(AnalyticFilter filter, CancellationToken cancellationToken);
    }
}
