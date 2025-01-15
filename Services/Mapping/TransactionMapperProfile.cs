using AutoMapper;
using Domain.Entities;
using DTOs.TransactionDTOs;

namespace Services.Mapping
{
    public class TransactionMapperProfile : Profile
    {
        public TransactionMapperProfile()
        {
            CreateMap<CreateTransactionDTO, Income>()
                .ForMember(dest => dest.IncomeDate, opt => opt.MapFrom(_ => _.TransactionDate))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<CreateTransactionDTO, Expense>()
                .ForMember(dest => dest.ExpenseDate, opt => opt.MapFrom(_ => _.TransactionDate))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateTransactionDTO, Income>()
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateTransactionDTO, Expense>()
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Income, TransactionDTO>()
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(_ => _.IncomeDate))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(_ => _.Category.Name))
                .ForMember(dest => dest.CategoryType, opt => opt.MapFrom(_ => _.Category.CategoryType))
                .ForMember(dest => dest.BudgetName, opt => opt.MapFrom(_ => _.Budget.Name));

            CreateMap<Expense, TransactionDTO>()
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(_ => _.ExpenseDate))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(_ => _.Category.Name))
                .ForMember(dest => dest.CategoryType, opt => opt.MapFrom(_ => _.Category.CategoryType))
                .ForMember(dest => dest.BudgetName, opt => opt.MapFrom(_ => _.Budget.Name));
        }
    }
}
