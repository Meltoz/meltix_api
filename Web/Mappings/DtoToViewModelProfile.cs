using Application.DTOs;
using AutoMapper;
using Web.Constantes;
using Web.ViewModels;

namespace Web.Mappings
{
    public class DtoToViewModelProfile : Profile
    {
        public DtoToViewModelProfile()
        {

            CreateMap<VideoDTO, VideoVM>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : ApiConstantes.NoCategory));

            CreateMap<CategoryDTO, CategoryVM>();

            CreateMap<VideoDTO, VideoCardVM>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : ApiConstantes.NoCategory));

            CreateMap<TagDTO, TagVM>();

        }
    }
}
