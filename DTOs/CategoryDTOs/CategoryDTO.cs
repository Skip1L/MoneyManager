﻿using Domain.Enums;

namespace DTOs.CategoryDTOs
{
    public class CategoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public CategoryType CategoryType { get; set; }
    }
}
