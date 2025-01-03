using AutoMapper;
using Domain.Entities;

namespace Services.DTOs.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Budget, BudgetDto>()
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src =>
                src.Incomes.Sum(i => i.Amount) - src.Expenses.Sum(e => e.Amount)));
            CreateMap<Expense, ExpenseDto>();
            CreateMap<Income, IncomeDto>();
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.CategoryType.ToString()));

            CreateMap<BudgetDto, Budget>();
            CreateMap<ExpenseDto, Expense>();
            CreateMap<IncomeDto, Income>();
            CreateMap<CategoryDto, Category>();
        }
    }
}
