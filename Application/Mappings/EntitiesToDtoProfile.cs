using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class EntitiesToDtoProfile : Profile
    {
        public EntitiesToDtoProfile()
        {
            CreateMap<Video, VideoDTO>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Value)));

            CreateMap<Category, CategoryDTO>();
        }
    }
}
