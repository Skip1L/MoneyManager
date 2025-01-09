﻿using Domain.Enums;

namespace DTOs.CommonDTOs
{
    public class UpdateCategoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public CategoryType CategoryType { get; set; }
    }
}
