using AutoMapper;
using Domain.Entities;
using DTOs.CategoryDTOs;
using DTOs.CommonDTOs;

namespace Services.Mapping
{
    public class CategoryMapperProfile : Profile
    {
        public CategoryMapperProfile()
        {
            CreateMap<CreateCategoryDTO, Category>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateCategoryDTO, Category>()
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Category, CategoryDTO>();

            CreateMap<Category, ShortCategoryDTO>();
        }
    }
}
