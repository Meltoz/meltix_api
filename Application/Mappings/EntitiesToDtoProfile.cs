using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class EntitiesToDtoProfile : Profile
    {
        public EntitiesToDtoProfile()
        {
            CreateMap<Video, VideoDTO>();
            CreateMap<Category, CategoryDTO>();


        }
    }
}
