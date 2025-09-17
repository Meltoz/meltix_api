using Application.DTOs;
using Shared;
using Shared.Enums;

namespace Application.Interfaces
{
    public interface IVideoService
    {
        public Task<VideoDTO> FindBySlugAsync(string slug);

        public Task<PagedResult<VideoDTO>> PaginateAsync(int pageIndex, int pageSize, string search, SearchScopeVideo scope = SearchScopeVideo.All);


        public Task<PagedResult<VideoDTO>> SearchRecommendationsAsync(int pageIndex, int pageSize, VideoDTO videoReference);

        public Task<PagedResult<VideoDTO>> GetLastestVideos(int pageIndex, int pageSize, int days);

        public Task SyncFolderWithDatabaseAsync();

        public Task<VideoDTO> UpdateVideoAsync(VideoDTO videoDTO);


    }
}
