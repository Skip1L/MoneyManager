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
                .ForMember(dest => dest.IncomeDate, opt => opt.MapFrom(income => income.TransactionDate))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<CreateTransactionDTO, Expense>()
                .ForMember(dest => dest.ExpenseDate, opt => opt.MapFrom(expense => expense.TransactionDate))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateTransactionDTO, Income>()
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateTransactionDTO, Expense>()
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Income, TransactionDTO>()
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(income => income.IncomeDate))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(income => income.Category.Name))
                .ForMember(dest => dest.CategoryType, opt => opt.MapFrom(income => income.Category.CategoryType))
                .ForMember(dest => dest.BudgetName, opt => opt.MapFrom(income => income.Budget.Name));

            CreateMap<Expense, TransactionDTO>()
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(expense => expense.ExpenseDate))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(expense => expense.Category.Name))
                .ForMember(dest => dest.CategoryType, opt => opt.MapFrom(expense => expense.Category.CategoryType))
                .ForMember(dest => dest.BudgetName, opt => opt.MapFrom(expense => expense.Budget.Name));
        }
    }
}
