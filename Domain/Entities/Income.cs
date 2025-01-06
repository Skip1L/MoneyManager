namespace Domain.Entities
{
    public class Income : IBaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public double Amount { get; set; }
        public DateTime IncomeDate { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public Guid BudgetId { get; set; }
        public virtual Budget Budget { get; set; }

    }
}
