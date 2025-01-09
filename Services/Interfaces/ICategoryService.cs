using DTOs.CategoryDTOs;
using DTOs.CommonDTOs;

namespace Services.Interfaces
{
    public interface ICategoryService
    {
        Task CreateCategoryAsync(CreateCategoryDTO categoryDTO);
        Task UpdateCategoryAsync(UpdateCategoryDTO categoryDTO);
        Task DeleteCategoryAsync(Guid categoryId);
        Task<List<ShortCategoryDTO>> FilterCategoryAsync(PagginationDTO paginationDto);
        Task<CategoryDTO> GetCategoryByIdAsync(Guid categoryId);
    }
}
