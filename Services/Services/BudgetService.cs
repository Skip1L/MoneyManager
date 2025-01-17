using AutoMapper;
using Domain.Entities;
using DTOs.BudgetDTOs;
using DTOs.CommonDTOs;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Services.RepositoryInterfaces;

namespace Services.Services
{
    public class BudgetService(IBudgetRepository budgetRepository, ILogger<BudgetService> logger, IMapper mapper) : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepository = budgetRepository;
        private readonly ILogger<BudgetService> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task CreateBudgetAsync(CreateBudgetDTO budgetDTO, Guid userId, CancellationToken cancellationToken)
        {
            var budget = _mapper.Map<Budget>(budgetDTO);
            budget.UserId = userId;

            await _budgetRepository.CreateAsync(budget, cancellationToken);
            await _budgetRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteBudgetAsync(Guid budgetId, Guid userId, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.FirstOrDefaultAsync(budget => budget.Id == budgetId && budget.UserId == userId);

            if (budget == null)
            {
                _logger.LogError($"Budget is not found. budgetId: {budgetId}; userId: {userId}");
                return;
            }

            _budgetRepository.Delete(budget);
            await _budgetRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ShortBudgetDTO>> FilterBudgetAsync(PaginationFilter paginationDto, Guid userId, CancellationToken cancellationToken)
        {
            var dbEntity = await _budgetRepository.GetPagedAsync(
                paginationDto.PageSize,
                paginationDto.PageNumber,
                budget => budget.UserId == userId
                    && (string.IsNullOrWhiteSpace(paginationDto.SearchFilter.SearchString) || budget.Name.Contains(paginationDto.SearchFilter.SearchString)),
                cancellationToken);

            return _mapper.Map<List<ShortBudgetDTO>>(dbEntity);
        }

        public async Task<BudgetDTO> GetBudgetByIdAsync(Guid budgetId, string userName, CancellationToken cancellationToken)
        {
            var dbEntity = await _budgetRepository.GetBudgetWithUserAsync(budgetId, cancellationToken);

            if (dbEntity.User.UserName != userName)
            {
                _logger.LogError($"Budget is not found. budgetId: {budgetId}; userName: {userName}");
                throw new Exception("Budget is not found");
            }

            return _mapper.Map<BudgetDTO>(dbEntity);
        }

        public async Task UpdateBudgetAsync(UpdateBudgetDTO budgetDTO, Guid userId, CancellationToken cancellationToken)
        {
            var dbEntity = await _budgetRepository.FirstOrDefaultAsync(
                budget => budget.Id == budgetDTO.Id && budget.UserId == userId,
                cancellationToken);

            if (dbEntity == null)
            {
                _logger.LogError($"Budget is not found. budgetId: {budgetDTO.Id}; userId: {userId}");
                return;
            }

            _mapper.Map(budgetDTO, dbEntity);

            _budgetRepository.Update(dbEntity);
            await _budgetRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
