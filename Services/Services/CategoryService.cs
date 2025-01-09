using AutoMapper;
using Domain.Entities;
using DTOs.CategoryDTOs;
using DTOs.CommonDTOs;
using Services.Interfaces;
using Services.RepositoryInterfaces;

namespace Services.Services
{
    public class CategoryService(IGenericRepository<Category> categoryRepository, IMapper mapper) : ICategoryService
    {
        private readonly IGenericRepository<Category> _categoryRepository = categoryRepository;
        private readonly IMapper _mapper = mapper;

        public async Task CreateCategoryAsync(CreateCategoryDTO categoryDTO)
        {
            var category = _mapper.Map<Category>(categoryDTO);

            await _categoryRepository.CreateAsync(category);
            await _categoryRepository.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Guid categoryId)
        {
            await _categoryRepository.DeleteAsync(categoryId);
            await _categoryRepository.SaveChangesAsync();
        }

        public async Task<List<ShortCategoryDTO>> FilterCategoryAsync(PagginationDTO paginationDto)
        {
            var categoryPage = await _categoryRepository.GetPagedAsync(paginationDto.PageSize, paginationDto.PageNumber, category => category.Name.Contains(paginationDto.SearchString));
            return _mapper.Map<List<ShortCategoryDTO>>(categoryPage);
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(Guid categoryId)
        {
            var dbEntity = await _categoryRepository.FirstOrDefaultAsync(category => category.Id == categoryId);
            return _mapper.Map<CategoryDTO>(dbEntity);
        }

        public async Task UpdateCategoryAsync(UpdateCategoryDTO categoryDto)
        {
            var dbEntity = await _categoryRepository.FirstOrDefaultAsync(category => category.Id == categoryDto.Id);

            if (dbEntity == null)
            {
                return;
            }

            _mapper.Map(categoryDto, dbEntity);

            _categoryRepository.Update(dbEntity);
            await _categoryRepository.SaveChangesAsync();
        }
    }
}
