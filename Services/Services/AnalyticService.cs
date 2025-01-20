using Domain.Enums;
using DTOs.AnalyticDTOs;
using Services.Interfaces;
using Services.RepositoryInterfaces;

namespace Services.Services
{
    public class AnalyticService(IBudgetRepository budgetRepository, IIncomeRepository incomeRepository, IExpenseRepository expenseRepository) : IAnalyticService
    {
        private readonly IIncomeRepository _incomeRepository = incomeRepository;
        private readonly IExpenseRepository _expenseRepository = expenseRepository;
        private readonly IBudgetRepository _budgetRepository = budgetRepository;

        public async Task<List<AnalyticDTO>> GetAnalyticsByFilter(AnalyticFilter filter, CancellationToken cancellationToken)
        {
            return filter.CategoryType switch
            {
                CategoryType.Income => await _incomeRepository.GetTotalIncomeByCategoriesAsync(filter, cancellationToken),
                CategoryType.Expense => await _expenseRepository.GetTotalExpenseByCategoriesAsync(filter, cancellationToken),
                _ => await _budgetRepository.GetTotalTransactionsByBudgetAsync(filter, cancellationToken),
            };
        }
    }
}
