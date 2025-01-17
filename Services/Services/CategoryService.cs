using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using DTOs.CategoryDTOs;
using DTOs.CommonDTOs;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Services.RepositoryInterfaces;

namespace Services.Services
{
    public class CategoryService(IGenericRepository<Category> categoryRepository, ILogger<BudgetService> logger, IMapper mapper) : ICategoryService
    {
        private readonly IGenericRepository<Category> _categoryRepository = categoryRepository;
        private readonly ILogger<BudgetService> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task CreateCategoryAsync(CreateCategoryDTO categoryDTO, CancellationToken cancellationToken)
        {
            var category = _mapper.Map<Category>(categoryDTO);

            await _categoryRepository.CreateAsync(category, cancellationToken);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteCategoryAsync(Guid categoryId, CancellationToken cancellationToken)
        {
            await _categoryRepository.DeleteAsync(categoryId, cancellationToken);
        }

        public async Task<List<ShortCategoryDTO>> FilterCategoryAsync(DataFilter dataFilter, CategoryType? categoryType, CancellationToken cancellationToken)
        {
            if (dataFilter?.PaginationFilter is null || dataFilter.SearchFilter is null)
            {
                _logger.LogError("Filter is empty");
                throw new ArgumentNullException("dataFilter is empty");
            }

            var categoryPage = await _categoryRepository.GetPagedAsync(
                dataFilter.PaginationFilter.PageSize,
                dataFilter.PaginationFilter.PageNumber,
                category => (string.IsNullOrWhiteSpace(dataFilter.SearchFilter.SearchString) || category.Name.Contains(dataFilter.SearchFilter.SearchString))
                    && (categoryType == null || category.CategoryType == categoryType),
                cancellationToken);

            return _mapper.Map<List<ShortCategoryDTO>>(categoryPage);
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(Guid categoryId, CancellationToken cancellationToken)
        {
            var dbEntity = await _categoryRepository.FirstOrDefaultAsync(category => category.Id == categoryId, cancellationToken);
            return _mapper.Map<CategoryDTO>(dbEntity);
        }

        public async Task UpdateCategoryAsync(UpdateCategoryDTO categoryDto, CancellationToken cancellationToken)
        {
            var dbEntity = await _categoryRepository.FirstOrDefaultAsync(category => category.Id == categoryDto.Id, cancellationToken);

            if (dbEntity == null)
            {
                _logger.LogError($"Category is not found. categoryId: {categoryDto.Id}; categoryType: {categoryDto.CategoryType}");
                return;
            }

            _mapper.Map(categoryDto, dbEntity);

            _categoryRepository.Update(dbEntity);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
