using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using DTOs.TransactionDTOs;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Services.RepositoryInterfaces;

namespace Services.Services
{
    public class TransactionService(IIncomeRepository incomeRepository, IExpenseRepository expenseRepository, IBudgetRepository budgetRepository, ILogger<TransactionService> logger, IMapper mapper) : ITransactionService
    {
        private readonly IIncomeRepository _incomeRepository = incomeRepository;
        private readonly IExpenseRepository _expenseRepository = expenseRepository;
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

        public async Task UpdateTransactionAsync(UpdateTransactionDTO transactionDTO, Guid userId, CancellationToken cancellationToken)
        {
            var updateExpenseTask = _expenseRepository.FirstOrDefaultAsync(
                expense => expense.Id == transactionDTO.Id && expense.Budget.UserId == userId,
                cancellationToken
            );

            var updateIncomeTask = _incomeRepository.FirstOrDefaultAsync(
                income => income.Id == transactionDTO.Id && income.Budget.UserId == userId,
                cancellationToken
            );

            await Task.WhenAll(updateExpenseTask, updateIncomeTask);

            var expense = updateExpenseTask.Result;
            var income = updateIncomeTask.Result;

            if (expense != null)
            {
                _mapper.Map(transactionDTO, expense);
                _expenseRepository.Update(expense);
                await _expenseRepository.SaveChangesAsync(cancellationToken);
                return;
            }

            if (income != null)
            {
                _mapper.Map(transactionDTO, income);
                _incomeRepository.Update(income);
                await _incomeRepository.SaveChangesAsync(cancellationToken);
                return;
            }

            _logger.LogError($"Transaction not found. transactionId: {transactionDTO.Id}; userId: {userId}");
        }

        public async Task DeleteTransactionAsync(Guid transactionId, Guid userId, CancellationToken cancellationToken)
        {
            var deleteExpenseTask = _expenseRepository.FirstOrDefaultAsync(
                expense => expense.Id == transactionId && expense.Budget.UserId == userId,
                cancellationToken
            );

            var deleteIncomeTask = _incomeRepository.FirstOrDefaultAsync(
                income => income.Id == transactionId && income.Budget.UserId == userId,
                cancellationToken
            );

            await Task.WhenAll(deleteExpenseTask, deleteIncomeTask);

            var expense = deleteExpenseTask.Result;
            var income = deleteIncomeTask.Result;

            if (expense != null)
            {
                _expenseRepository.Delete(expense);
                await _expenseRepository.SaveChangesAsync(cancellationToken);
                return;
            }

            if (income != null)
            {
                _incomeRepository.Delete(income);
                await _incomeRepository.SaveChangesAsync(cancellationToken);
                return;
            }

            _logger.LogError($"Transaction not found. transactionId: {transactionId}; userName: {userId}");
        }

        public async Task<TransactionDTO> GetTransactionByIdAsync(Guid transactionId, Guid userId, CancellationToken cancellationToken)
        {
            var getExpenseTask = _expenseRepository.GetExpenseWithBudgetAndCategoryAsync(transactionId, cancellationToken);
            var getIncomeTask = _incomeRepository.GetIncomeWithBudgetAndCategoryAsync(transactionId, cancellationToken);

            await Task.WhenAll(getExpenseTask, getIncomeTask);

            var expense = getExpenseTask.Result;
            var income = getIncomeTask.Result;

            if (expense != null && expense.Budget.UserId == userId)
            {
                return _mapper.Map<TransactionDTO>(expense);
            }

            if (income != null && income.Budget.UserId == userId)
            {
                return _mapper.Map<TransactionDTO>(income);
            }
            
            _logger.LogError($"Transaction not found. transactionId: {transactionId}; userId: {userId}");
            throw new Exception("Transaction not found");
        }

        public async Task<TransactionsSummaryDTO> GetTransactionsSummary(TransactionFilter filter, CancellationToken cancellationToken)
        {
            return await _budgetRepository.GetTransactionsSummaryByDateRange(filter, cancellationToken);
        }
    }
}
