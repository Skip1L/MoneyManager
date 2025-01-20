using DTOs.AnalyticDTOs;

namespace Services.Interfaces
{
    public interface IAnalyticService
    {
        Task<List<AnalyticDTO>> GetAnalyticsByFilter(AnalyticFilter filter, CancellationToken cancellationToken);
    }
}
