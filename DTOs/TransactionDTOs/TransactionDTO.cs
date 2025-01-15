using Domain.Enums;

namespace DTOs.TransactionDTOs
{
    public class TransactionDTO
    {
        public Guid Id { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string BudgetName { get; set; }
        public CategoryType CategoryType { get; set; }
    }
}
