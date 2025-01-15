using DTOs.CommonDTOs;

namespace DTOs.TransactionDTOs
{
    public class TransactionFilter
    {
        public Guid UserId { get; set; }
        public DateRangePaginationDTO DateRangePaginationDTO { get; set; }
    }
}
