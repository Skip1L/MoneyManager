using Domain.Enums;
using DTOs.CommonDTOs;

namespace DTOs.AnalyticDTOs
{
    public class AnalyticFilter
    {
        public Guid UserId { get; set; }
        public CategoryType? CategoryType { get; set; }
        public PaginationFilter PaginationFilter { get; set; }
        public DateRangeFilter DateRangeFilter { get; set; }
    }
}
