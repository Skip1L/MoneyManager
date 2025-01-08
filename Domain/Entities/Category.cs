using Domain.Enums;

namespace Domain.Entities
{
    public class Category : IBaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public CategoryType CategoryType { get; set; }
        public virtual ICollection<Expense> Expenses { get; set; }
        public virtual ICollection<Income> Incomes { get; set; }

    }
}
