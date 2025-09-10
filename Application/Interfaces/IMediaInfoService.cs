using Application.DTOs;

namespace Application.Interfaces
{
    public interface IMediaInfoService
    {
        public Task<MediaInfoDTO> GetMediaInfoAsync(string videoPath, CancellationToken cancellationToken=default);
    }
}
