﻿using Domain.Enums;

namespace DTOs.TransactionDTOs
{
    public class CreateTransactionDTO
    {
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid BudgetId { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public CategoryType CategoryType { get; set; }
    }
}
