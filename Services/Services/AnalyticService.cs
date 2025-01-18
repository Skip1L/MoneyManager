using Domain.Enums;
using DTOs.AnalyticDTOs;
using Services.Interfaces;
using Services.RepositoryInterfaces;

namespace Services.Services
{
    public class AnalyticService(IBudgetRepository budgetRepository, ICategoryRepository categoryRepository) : IAnalyticService
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IBudgetRepository _budgetRepository = budgetRepository;

        public async Task<List<AnalyticDTO>> GetAnalyticsByFilter(AnalyticFilter filter, CancellationToken cancellationToken)
        {
            switch (filter.CategoryType)
            {
                case CategoryType.Income:
                    return await _categoryRepository.GetIncomeAnalyticsByFilter(filter, cancellationToken);
                case CategoryType.Expense:
                    return await _categoryRepository.GetExpenseAnalyticsByFilter(filter, cancellationToken);
                default:
                    return await _budgetRepository.GetBudgetsAnalyticByFilter(filter, cancellationToken);
            }
        }
    }
}
