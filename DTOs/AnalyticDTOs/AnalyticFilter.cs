using Domain.Enums;
using DTOs.CommonDTOs;

namespace DTOs.AnalyticDTOs
{
    public class AnalyticFilter
    {
        public Guid UserId { get; set; }
        public CategoryType? CategoryType { get; set; }
        public DataFilter DataFilter { get; set; }
    }
}
