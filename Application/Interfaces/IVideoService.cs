using Application.DTOs;
using Shared;
using Shared.Enums;

namespace Application.Interfaces
{
    public interface IVideoService
    {
        public Task<VideoDTO> FindBySlugAsync(string slug);

        public Task<PagedResult<VideoDTO>> PaginateAsync(int pageIndex, int pageSize, string search, SortOption<SortVideo> sortOption, SearchScopeVideo scope = SearchScopeVideo.All);


        public Task<PagedResult<VideoDTO>> SearchRecommendationsAsync(int pageIndex, int pageSize, VideoDTO videoReference);

        public Task SyncFolderWithDatabaseAsync();

        public Task<VideoDTO> UpdateVideoAsync(VideoDTO videoDTO);


    }
}
