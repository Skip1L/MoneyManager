using Domain.Entities;

namespace Services.DTOs
{
    public class IncomeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime IncomeDate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
    }
}
