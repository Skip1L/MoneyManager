using AutoMapper;
using DTOs.CommonDTOs;
using DTOs.NotidicationDTOs;
using DTOs.TransactionDTOs;
using Google.Protobuf.WellKnownTypes;
using Services.Protos;

namespace Services.Mapping
{
    public class GrpcMappingProfile : Profile
    {
        public GrpcMappingProfile()
        {
            CreateMap<CategoryReportDTO, CategoryReport>();
            CreateMap<BudgetReportDTO, BudgetReport>();
            CreateMap<TransactionsSummaryDTO, TransactionsSummary>();

            CreateMap<DateRangeFilter, DateRange>()
                .ForMember(dest => dest.From, opt => opt.MapFrom(src => Timestamp.FromDateTimeOffset(new DateTimeOffset(src.From!.Value.Ticks, TimeSpan.Zero))))
                .ForMember(dest => dest.To, opt => opt.MapFrom(src => Timestamp.FromDateTimeOffset(new DateTimeOffset(src.To!.Value.Ticks, TimeSpan.Zero))));

            CreateMap<AnalyticEmailRequestDTO, SendEmailRequest>();
        }
    }
}
