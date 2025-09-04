using Application.DTOs;
using AutoMapper;
using Web.ViewModels;

namespace Web.Mappings
{
    public class DtoToViewModelProfile : Profile
    {
        public DtoToViewModelProfile()
        {

            CreateMap<VideoDTO, VideoVM>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => string.Join(';', src.Tags)))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "Uncategorised"));

            CreateMap<CategoryDTO, CategoryVM>();

            CreateMap<VideoDTO, VideoCardVM>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "No Category"));
        }
    }
}
