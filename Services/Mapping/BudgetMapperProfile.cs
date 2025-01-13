using AutoMapper;
using Domain.Entities;
using DTOs.BudgetDTOs;

namespace Services.Mapping
{
    public class BudgetMapperProfile : Profile
    {
        public BudgetMapperProfile()
        {
            CreateMap<CreateBudgetDTO, Budget>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
            
            CreateMap<UpdateBudgetDTO, Budget>()
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Budget, BudgetDTO>();

            CreateMap<Budget, ShortBudgetDTO>();
        }

    }
}
