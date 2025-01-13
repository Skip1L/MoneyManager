using AutoMapper;
using Domain.Entities;
using DTOs.BudgetDTOs;
using DTOs.CommonDTOs;
using Services.Interfaces;
using Services.RepositoryInterfaces;

namespace Services.Services
{
    public class BudgetService(IGenericRepository<Budget> budgetRepository, IMapper mapper) : IBudgetService
    {
        private readonly IGenericRepository<Budget> _budgetRepository = budgetRepository;
        private readonly IMapper _mapper = mapper;

        public async Task CreateBudgetAsync(CreateBudgetDTO budgetDTO, Guid userId, CancellationToken cancellationToken)
        {
            var budget = _mapper.Map<Budget>(budgetDTO);
            budget.UserId = userId;

            await _budgetRepository.CreateAsync(budget, cancellationToken);
            await _budgetRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteBudgetAsync(Guid budgetId, CancellationToken cancellationToken)
        {
            await _budgetRepository.DeleteAsync(budgetId, cancellationToken);
            await _budgetRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ShortBudgetDTO>> FilterBudgetAsync(PaginationDTO paginationDto, CancellationToken cancellationToken)
        {
            var dbEntity = await _budgetRepository.GetPagedAsync(paginationDto.PageSize, paginationDto.PageNumber, budget => budget.Name.Contains(paginationDto.SearchString), cancellationToken);
            return _mapper.Map<List<ShortBudgetDTO>>(dbEntity);
        }

        public async Task<BudgetDTO> GetBudgetByIdAsync(Guid budgetId, CancellationToken cancellationToken)
        {
            var dbEntity = await _budgetRepository.FirstOrDefaultAsync(budget => budget.Id == budgetId, cancellationToken);
            return _mapper.Map<BudgetDTO>(dbEntity);
        }

        public async Task UpdateBudgetAsync(UpdateBudgetDTO budgetDTO, CancellationToken cancellationToken)
        {
            var dbEntity = await _budgetRepository.FirstOrDefaultAsync(budget => budget.Id == budgetDTO.Id, cancellationToken);

            if (dbEntity == null)
            {
                return;
            }

            _mapper.Map(budgetDTO, dbEntity);

            _budgetRepository.Update(dbEntity);
            await _budgetRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
