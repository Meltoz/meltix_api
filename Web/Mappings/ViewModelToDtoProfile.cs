using Application.DTOs;
using AutoMapper;
using Web.ViewModels;

namespace Web.Mappings
{
    public class ViewModelToDtoProfile : Profile
    {
        public ViewModelToDtoProfile()
        {
            CreateMap<VideoRequestVM, VideoDTO>();
            CreateMap<CategoryVM, CategoryDTO>();
        }
    }
}
