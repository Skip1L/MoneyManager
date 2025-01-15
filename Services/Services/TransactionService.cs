using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using DTOs.TransactionDTOs;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Services.RepositoryInterfaces;

namespace Services.Services
{
    public class TransactionService(IGenericRepository<Income> incomeRepository, IGenericRepository<Expense> expenseRepository, IBudgetRepository budgetRepository, ILogger<TransactionService> logger, IMapper mapper) : ITransactionService
    {
        private readonly IGenericRepository<Income> _incomeRepository = incomeRepository;
        private readonly IGenericRepository<Expense> _expenseRepository = expenseRepository;
        private readonly IBudgetRepository _budgetRepository = budgetRepository;
        private readonly ILogger<TransactionService> _logger = logger;
        private readonly IMapper _mapper = mapper;

        public async Task CreateTransactionAsync(CreateTransactionDTO transactionDTO, CancellationToken cancellationToken)
        {
            switch (transactionDTO.CategoryType)
            {
                case CategoryType.Expense:
                    await CreateExpenseAsync(transactionDTO, cancellationToken);
                    break;
                case CategoryType.Income:
                    await CreateIncomeAsync(transactionDTO, cancellationToken);
                    break;
            }
        }

        private async Task CreateIncomeAsync(CreateTransactionDTO transactionDTO, CancellationToken cancellationToken)
        {
            var income = _mapper.Map<Income>(transactionDTO);

            await _incomeRepository.CreateAsync(income, cancellationToken);
            await _incomeRepository.SaveChangesAsync(cancellationToken);
        }

        private async Task CreateExpenseAsync(CreateTransactionDTO transactionDTO, CancellationToken cancellationToken)
        {
            var expense = _mapper.Map<Expense>(transactionDTO);

            await _expenseRepository.CreateAsync(expense, cancellationToken);
            await _expenseRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<TransactionDTO>> GetByFilterAsync(TransactionFilter filter, CancellationToken cancellationToken)
        {
            return await _budgetRepository.GetTransactionsByTransactionFilter(filter, cancellationToken);
        }

        public async Task UpdateTransactionAsync(UpdateTransactionDTO transactionDTO, CancellationToken cancellationToken)
        {
            switch (transactionDTO.CategoryType)
            {
                case CategoryType.Expense:
                    await UpdateExpenseAsync(transactionDTO, cancellationToken);
                    break;
                case CategoryType.Income:
                    await UpdateIncomeAsync(transactionDTO, cancellationToken);
                    break;
            }
        }

        private async Task UpdateIncomeAsync(UpdateTransactionDTO transactionDTO, CancellationToken cancellationToken)
        {
            var dbEntity = await _incomeRepository.FirstOrDefaultAsync(
                income => income.Id == transactionDTO.Id && income.Budget.UserId == transactionDTO.UserId,
                cancellationToken);

            if (dbEntity == null)
            {
                _logger.LogError($"Income is not found. budgetId: {transactionDTO.Id}; userId: {transactionDTO.UserId}");
                return;
            }

            _mapper.Map(transactionDTO, dbEntity);

            _incomeRepository.Update(dbEntity);
            await _incomeRepository.SaveChangesAsync(cancellationToken);
        }

        private async Task UpdateExpenseAsync(UpdateTransactionDTO transactionDTO, CancellationToken cancellationToken)
        {
            var dbEntity = await _expenseRepository.FirstOrDefaultAsync(
                expense => expense.Id == transactionDTO.Id && expense.Budget.UserId == transactionDTO.UserId,
                cancellationToken);

            if (dbEntity == null)
            {
                _logger.LogError($"Expense is not found. budgetId: {transactionDTO.Id}; userId: {transactionDTO.UserId}");
                return;
            }

            _mapper.Map(transactionDTO, dbEntity);

            _expenseRepository.Update(dbEntity);
            await _expenseRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteTransactionAsync(Guid transactionId, CategoryType categoryType, string userName, CancellationToken cancellationToken)
        {
            switch (categoryType)
            {
                case CategoryType.Expense:
                    await DeleteExpenseAsync(transactionId, userName, cancellationToken);
                    break;
                case CategoryType.Income:
                    await DeleteIncomeAsync(transactionId, userName, cancellationToken);
                    break;
            }
        }

        private async Task DeleteIncomeAsync(Guid transactionId, string userName, CancellationToken cancellationToken)
        {
            var income = await _incomeRepository.FirstOrDefaultAsync(income => income.Id == transactionId && income.Budget.User.UserName == userName, cancellationToken);

            if (income == null)
            {
                _logger.LogError($"Income is not found. budgetId: {transactionId}; userName: {userName}");
                return;
            }

            _incomeRepository.Delete(income);
            await _incomeRepository.SaveChangesAsync(cancellationToken);
        }

        private async Task DeleteExpenseAsync(Guid transactionId, string userName, CancellationToken cancellationToken)
        {
            var expense = await _expenseRepository.FirstOrDefaultAsync(expense => expense.Id == transactionId && expense.Budget.User.UserName == userName, cancellationToken);

            if (expense == null)
            {
                _logger.LogError($"Expense is not found. budgetId: {transactionId}; userName: {userName}");
                return;
            }

            _expenseRepository.Delete(expense);
            await _expenseRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task<TransactionDTO> GetTransactionByIdAsync(Guid transactionId, CategoryType categoryType, Guid userId, CancellationToken cancellationToken)
        {
            switch (categoryType)
            {
                case CategoryType.Expense:
                    return await GetExpenseByIdAsync(transactionId, userId, cancellationToken);
                case CategoryType.Income:
                    return await GetIncomeByIdAsync(transactionId, userId, cancellationToken);
                default:
                    return new TransactionDTO();
            }
        }

        private async Task<TransactionDTO> GetExpenseByIdAsync(Guid transactionId, Guid userId, CancellationToken cancellationToken)
        {
            var dbEntity = await _budgetRepository.GetExpenseWithBudgetAsync(transactionId, cancellationToken);

            if (dbEntity.Budget.UserId != userId)
            {
                _logger.LogError($"Expense is not found. transactionId: {transactionId}; userId: {userId}");
                throw new Exception("Budget is not found");
            }

            return _mapper.Map<TransactionDTO>(dbEntity);
        }

        private async Task<TransactionDTO> GetIncomeByIdAsync(Guid transactionId, Guid userId, CancellationToken cancellationToken)
        {
            var dbEntity = await _budgetRepository.GetIncomeWithBudgetAsync(transactionId, cancellationToken);

            if (dbEntity.Budget.UserId != userId)
            {
                _logger.LogError($"Income is not found. transactionId: {transactionId}; userId: {userId}");
                throw new Exception("Budget is not found");
            }

            return _mapper.Map<TransactionDTO>(dbEntity);
        }

        public async Task<TransactionsSummaryDTO> GetTransactionsSummary(TransactionSummaryFilter filter, CancellationToken cancellationToken)
        {
            return await _budgetRepository.GetTransactionsSummaryByDateRange(filter, cancellationToken);
        }
    }
}
