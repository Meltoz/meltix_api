using Application.DTOs;
using Shared;

namespace Application.Interfaces
{
    public interface IVideoService
    {
        public Task<ServiceResponse<(IEnumerable<VideoDTO> videos, int totalCount)>> Paginate(int pageIndex, int pageSize, string search);

        public Task SyncFolderWithDatabaseAsync();

        public Task<ServiceResponse<VideoDTO>> UpdateVideo(VideoDTO videoDTO);
    }
}
