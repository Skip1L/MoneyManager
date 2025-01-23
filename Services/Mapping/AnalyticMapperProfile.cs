using AutoMapper;
using DTOs.AnalyticDTOs;
using DTOs.NotidicationDTOs;

namespace Services.Mapping
{
    public class AnalyticMapperProfile : Profile
    {
        public AnalyticMapperProfile() 
        {
            CreateMap<AnalyticDTO, CategoryReportDTO>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(a => a.TotalIncome > a.TotalExpense ? a.TotalIncome : a.TotalExpense));

            CreateMap<AnalyticDTO, BudgetReportDTO>()
                .ForMember(dest => dest.Income, opt => opt.MapFrom(a => a.TotalIncome))
                .ForMember(dest => dest.Expense, opt => opt.MapFrom(a => a.TotalExpense));
        }
    }
}
