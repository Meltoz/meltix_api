using Application.DTOs;
using AutoMapper;
using Web.ViewModels;

namespace Web.Mappings
{
    public class ViewModelToDtoProfile : Profile
    {
        public ViewModelToDtoProfile()
        {
            CreateMap<VideoRequestVM, VideoDTO>()
                .ForMember(dest => dest.Duration, opt => opt.Ignore())
                .ForMember(dest => dest.Path, opt => opt.Ignore());

            CreateMap<CategoryVM, CategoryDTO>();
        }
    }
}
