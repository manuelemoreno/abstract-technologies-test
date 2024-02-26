using AutoMapper;
using AwesomeFruits.Application.DTOs;
using AwesomeFruits.Domain.Entities;

namespace AwesomeFruits.Application.Mapping.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest =>
                    dest.UserName,
                opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest =>
                    dest.Email,
                opt => opt.MapFrom(src => src.Email))
            .ForMember(dest =>
                    dest.Id,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest =>
                    dest.FirstName,
                opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest =>
                    dest.LastName,
                opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest =>
                    dest.IsActive,
                opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest =>
                    dest.PasswordHash,
                opt => opt.MapFrom(src => src.PasswordHash))
            .ForMember(dest =>
                    dest.PasswordSalt,
                opt => opt.MapFrom(src => src.PasswordSalt))
            ;
    }
}