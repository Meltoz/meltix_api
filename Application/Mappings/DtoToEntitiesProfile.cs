using Application.DTOs;
using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Domain.Entities;
using Domain.Enums;

namespace Application.Mappings
{
    public class DtoToEntitiesProfile : Profile
    {
       public DtoToEntitiesProfile()
        {
            CreateMap<VideoDTO, Video>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Slug, opt => opt.Ignore())
                .ForMember(dest => dest.Title, opt => opt.Ignore())
                .ForMember(dest => dest.Duration, opt => opt.Ignore())
                .ForMember(dest => dest.Path, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Category.Id));

            CreateMap<RoleDTO, Role>()
                    .ConvertUsingEnumMapping(opt => opt.MapByName().MapValue(RoleDTO.Admin, Role.Administrator));

            CreateMap<UserDTO, User>();


        }
    }
}
