using Application.DTOs;
using AutoMapper;
using Web.ViewModels;

namespace Web.Mappings
{
    public class ViewModelToDtoProfile : Profile
    {
        public ViewModelToDtoProfile()
        {
            CreateMap<VideoRequestVM, UpdateVideoDTO>()
             .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()))
             .ForMember(dest => dest.Timecode, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Timecode) ? TimeSpan.Zero : TimeSpan.Parse(src.Timecode)))
             .ForMember(dest => dest.ThumbnailFile, opt => opt.MapFrom(src => src.Img == null ? null : src.Img))
             .ForMember(dest => dest.Duration, opt => opt.Ignore())
             .ForMember(dest => dest.Path, opt => opt.Ignore())
             .ForMember(dest => dest.Category, opt => opt.Ignore());

            CreateMap<CategoryVM, CategoryDTO>();
        }
    }
}
