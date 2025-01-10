using DTOs.BudgetDTOs;
using DTOs.CommonDTOs;

namespace Services.Interfaces
{
    public interface IBudgetService
    {
        Task CreateBudgetAsync(CreateBudgetDTO BudgetDTO, Guid userId, CancellationToken cancellationToken);
        Task UpdateBudgetAsync(BudgetDTO BudgetDTO, CancellationToken cancellationToken);
        Task DeleteBudgetAsync(Guid BudgetId, CancellationToken cancellationToken);
        Task<List<ShortBudgetDTO>> FilterBudgetAsync(PagginationDTO paginationDto, CancellationToken cancellationToken);
        Task<BudgetDTO> GetBudgetByIdAsync(Guid BudgetId, CancellationToken cancellationToken);
    }
}
