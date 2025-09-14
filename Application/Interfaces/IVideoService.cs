using Application.DTOs;
using Shared;
using Shared.Enums;

namespace Application.Interfaces
{
    public interface IVideoService
    {
        public Task<VideoDTO> FindBySlugAsync(string slug);

        public Task<(IEnumerable<VideoDTO> videos, int totalCount)> PaginateAsync(int pageIndex, int pageSize, string search, SearchScopeVideo scope = SearchScopeVideo.All);


        public Task<(IEnumerable<VideoDTO> videos, int totalCount)> SearchRecommendationsAsync(int pageIndex, int pageSize, VideoDTO videoReference);

        public Task SyncFolderWithDatabaseAsync();

        public Task<VideoDTO> UpdateVideoAsync(VideoDTO videoDTO);


    }
}
