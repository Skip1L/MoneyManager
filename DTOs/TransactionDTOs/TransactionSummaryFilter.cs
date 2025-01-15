using DTOs.CommonDTOs;

namespace DTOs.TransactionDTOs
{
    public class TransactionSummaryFilter
    {
        public Guid UserId { get; set; }
        public DateRangeDTO DateRange { get; set; }
    }
}
