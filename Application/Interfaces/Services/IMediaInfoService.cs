using Application.DTOs;

namespace Application.Interfaces.Services
{
    public interface IMediaInfoService
    {
        public Task<MediaInfoDTO> GetMediaInfoAsync(string videoPath, CancellationToken cancellationToken=default);
    }
}
