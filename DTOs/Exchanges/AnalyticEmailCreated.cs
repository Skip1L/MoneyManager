using DTOs.CommonDTOs;
using DTOs.NotidicationDTOs;
using DTOs.TransactionDTOs;

namespace DTOs.Exchanges
{
    public interface AnalyticEmailCreated
    {
        public Guid Id { get; set; }
        public string RecipientName { get; set; }
        public string ToEmail { get; set; }
        public List<CategoryReportDTO> Incomes { get; set; }
        public List<CategoryReportDTO> Expenses { get; set; }
        public List<BudgetReportDTO> Budgets { get; set; }
        public TransactionsSummaryDTO TransactionsSummary { get; set; }
        public DateRangeFilter DateRange { get; set; }
    }
}
