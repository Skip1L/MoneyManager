namespace Services.DTOs
{
    public class ExpenseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime ExpenseDate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
    }
}
