using DTOs.BudgetDTOs;
using DTOs.CommonDTOs;

namespace Services.Interfaces
{
    public interface IBudgetService
    {
        Task CreateBudgetAsync(CreateBudgetDTO BudgetDTO, Guid userId, CancellationToken cancellationToken);
        Task UpdateBudgetAsync(UpdateBudgetDTO BudgetDTO, Guid userId, CancellationToken cancellationToken);
        Task DeleteBudgetAsync(Guid BudgetId, Guid userId, CancellationToken cancellationToken);
        Task<List<ShortBudgetDTO>> FilterBudgetAsync(PaginationFilter paginationDto, Guid userId, CancellationToken cancellationToken);
        Task<BudgetDTO> GetBudgetByIdAsync(Guid BudgetId, string userName, CancellationToken cancellationToken);
    }
}
