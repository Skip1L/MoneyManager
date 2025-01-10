using DTOs.CategoryDTOs;
using DTOs.CommonDTOs;

namespace Services.Interfaces
{
    public interface ICategoryService
    {
        Task CreateCategoryAsync(CreateCategoryDTO categoryDTO, CancellationToken cancellationToken);
        Task UpdateCategoryAsync(UpdateCategoryDTO categoryDTO, CancellationToken cancellationToken);
        Task DeleteCategoryAsync(Guid categoryId, CancellationToken cancellationToken);
        Task<List<ShortCategoryDTO>> FilterCategoryAsync(PagginationDTO paginationDto, CancellationToken cancellationToken);
        Task<CategoryDTO> GetCategoryByIdAsync(Guid categoryId, CancellationToken cancellationToken);
    }
}
