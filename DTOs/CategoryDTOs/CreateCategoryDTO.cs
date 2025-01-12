using Domain.Enums;

namespace DTOs.CategoryDTOs
{
    public class CreateCategoryDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public CategoryType CategoryType { get; set; }
    }
}
