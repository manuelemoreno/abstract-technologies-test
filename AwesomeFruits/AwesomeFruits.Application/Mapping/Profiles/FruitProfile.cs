using AutoMapper;
using AwesomeFruits.Application.DTOs;
using AwesomeFruits.Domain.Entities;

namespace AwesomeFruits.Application.Mapping.Profiles;

public class FruitProfile : Profile
{
    public FruitProfile()
    {
        CreateMap<Fruit, FruitDto>()
            .ForMember(dest =>
                    dest.Name,
                opt => opt.MapFrom(src => src.Name))
            .ForMember(dest =>
                    dest.Description,
                opt => opt.MapFrom(src => src.Description))
            .ForMember(dest =>
                    dest.CreatedAtUtc,
                opt => opt.MapFrom(src => src.CreatedAtUtc))
            .ForMember(dest =>
                    dest.LastUpdatedAtUtc,
                opt => opt.MapFrom(src => src.LastUpdatedAtUtc))
            .ForMember(dest =>
                    dest.CreatedByUserId,
                opt => opt.MapFrom(src => src.CreatedByUserId))
            .ForMember(dest =>
                    dest.Id,
                opt => opt.MapFrom(src => src.Id));
    }
}