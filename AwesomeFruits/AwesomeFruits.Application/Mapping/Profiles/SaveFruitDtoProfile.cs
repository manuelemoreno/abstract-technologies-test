using System;
using AutoMapper;
using AwesomeFruits.Application.DTOs;
using AwesomeFruits.Domain.Entities;

namespace AwesomeFruits.Application.Mapping.Profiles;

public class SaveFruitDtoProfile : Profile
{
    public SaveFruitDtoProfile()
    {
        CreateMap<SaveFruitDto, Fruit>(MemberList.None)
            .ForMember(dest =>
                    dest.Name,
                opt => opt.MapFrom(src => src.Name))
            .ForMember(dest =>
                    dest.CreatedByUserId,
                opt => opt.MapFrom(src => src.CreatedByUserId))
            .ForMember(dest =>
                    dest.Description,
                opt => opt.MapFrom(src => src.Description))
            .AfterMap((src, dest) =>
            {
                var createdTimeAt = DateTime.UtcNow;

                dest.CreatedAtUtc = createdTimeAt;
                dest.LastUpdatedAtUtc = createdTimeAt;
            });
    }
}