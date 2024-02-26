using AutoMapper;
using AwesomeFruits.Application.DTOs;
using AwesomeFruits.Domain.Entities;

namespace AwesomeFruits.Application.Mapping.Profiles;

public class SaveUserDtoProfile : Profile
{
    public SaveUserDtoProfile()
    {
        CreateMap<SaveUserDto, User>()
            .ForMember(dest =>
                    dest.UserName,
                opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest =>
                    dest.Email,
                opt => opt.MapFrom(src => src.Email))
            .ForMember(dest =>
                    dest.FirstName,
                opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest =>
                    dest.LastName,
                opt => opt.MapFrom(src => src.LastName))
            ;
    }
}