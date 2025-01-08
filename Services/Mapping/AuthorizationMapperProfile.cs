using AutoMapper;
using Domain.Entities;
using DTOs.AuthorizationDTOs;

namespace Services.Mapping
{
    public class AuthorizationMapperProfile : Profile
    {
        public AuthorizationMapperProfile()
        {
            CreateMap<SignUpDTO, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.LastUpdatedAt, opt => opt.Ignore());
        }
    }
}
