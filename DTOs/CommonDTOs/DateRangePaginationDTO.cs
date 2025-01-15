namespace DTOs.CommonDTOs
{
    public class DateRangePaginationDTO
    {
        public PaginationDTO Pagination { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
