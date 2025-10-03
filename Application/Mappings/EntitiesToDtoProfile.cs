using Application.DTOs;
using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Domain.Entities;
using Domain.Enums;

namespace Application.Mappings
{
    public class EntitiesToDtoProfile : Profile
    {
        public EntitiesToDtoProfile()
        {
            CreateMap<Video, VideoDTO>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Value)));

            CreateMap<Category, CategoryDTO>();

            CreateMap<Tag, TagDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.VideoCount, opt => opt.MapFrom(src => src.Videos.Count))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Value));

            CreateMap<Role, RoleDTO>()
                .ConvertUsingEnumMapping(opt => opt.MapByName().MapValue(Role.Administrator, RoleDTO.Admin));

            CreateMap<User, UserDTO>();

            CreateMap<TokenInfo, TokenDTO>()
                .ForMember(dest => dest.RefreshExpiresAt, opt => opt.MapFrom(src => src.ExpiresAt))
                .ForMember(dest => dest.AccessToken, opt => opt.MapFrom(src => src.AccessToken.Value))
                .ForMember(dest => dest.RefreshToken, opt => opt.MapFrom(src => src.RefreshToken.Value));
        }
    }
}
