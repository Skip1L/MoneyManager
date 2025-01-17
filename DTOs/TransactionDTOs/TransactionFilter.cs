using DTOs.CommonDTOs;

namespace DTOs.TransactionDTOs
{
    public class TransactionFilter
    {
        public Guid UserId { get; set; }
        public PaginationFilter Pagination { get; set; }
        public DateRangeFilter DateRange { get; set; }
    }
}
