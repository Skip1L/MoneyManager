namespace Domain.Entities
{
    public class Income : IBaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public decimal Amount { get; set; }
        public DateTime IncomeDate { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
        public Guid BudgetId { get; set; }
        public virtual Category Category { get; set; }
        public virtual Budget Budget { get; set; }

    }
}
