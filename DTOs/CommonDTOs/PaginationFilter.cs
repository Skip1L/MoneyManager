namespace DTOs.CommonDTOs
{
    public class PaginationFilter
    {
        public SearchFilter SearchFilter { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
