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

        public async Task CreateCategoryAsync(CreateCategoryDTO categoryDTO, CancellationToken cancellationToken)
        {
            var category = _mapper.Map<Category>(categoryDTO);

            await _categoryRepository.CreateAsync(category, cancellationToken);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteCategoryAsync(Guid categoryId, CancellationToken cancellationToken)
        {
            await _categoryRepository.DeleteAsync(categoryId, cancellationToken);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ShortCategoryDTO>> FilterCategoryAsync(PaginationDTO paginationDto, CancellationToken cancellationToken)
        {
            var categoryPage = await _categoryRepository.GetPagedAsync(paginationDto.PageSize, paginationDto.PageNumber, category => category.Name.Contains(paginationDto.SearchString), cancellationToken);
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
                return;
            }

            _mapper.Map(categoryDto, dbEntity);

            _categoryRepository.Update(dbEntity);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
