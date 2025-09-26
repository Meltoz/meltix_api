using Domain.Entities;
using Shared;
using Shared.Enums.Sorting.Video;

namespace Application.Interfaces.Repository
{
    public interface IVideoRepository : IRepository<Video>
    {
        public Task<Video?> GetBySlug(string slug);

        public Task<(IEnumerable<Video> videos, int totalCount)> Search(int skip, int take, string search, SortOption<SortVideo> sortOption, SearchScopeVideo scope = SearchScopeVideo.All);

        public Task<(IEnumerable<Video> videos, int totalCount)> GetRecommendation(int skip, int take, Video reference);

        public Task InsertRangeAsync(IEnumerable<Video> batch);

        public new Task<Video?> GetByIdAsync(Guid id);
    }
}
