﻿using Domain.Enums;

namespace DTOs.CategoryDTOs
{
    public class ShortCategoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public CategoryType CategoryType { get; set; }
    }
}
