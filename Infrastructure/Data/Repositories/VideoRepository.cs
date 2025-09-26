using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared;
using Application.Interfaces.Repository;
using Shared.Enums.Sorting;
using Shared.Enums.Sorting.Video;

namespace Infrastructure.Data.Repositories
{
    public class VideoRepository(MeltixContext context) : GenericRepository<Video>(context), IVideoRepository
    {

        public async Task<Video?> GetBySlug(string slug)
        {
            return await _dbSet
                .Include(v => v.Category)
                .Include(v => v.Tags)
                .Where(v => v.Slug == slug).FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<Video> videos, int totalCount)> Search(int skip, int take, string search, SortOption<SortVideo> sortOption, SearchScopeVideo scope = SearchScopeVideo.All)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(v => v.Category)
                .Include(v => v.Tags)
                .AsQueryable();

            if (scope != SearchScopeVideo.Uncategorised)
            {
                query = query.Where(v => v.Category != null);
            }

            switch (scope)
            {
                case SearchScopeVideo.TitleDescription:
                    query = query.Where(v => EF.Functions.Like(v.Title, search) ||
                                                  EF.Functions.Like(v.Description, search));

                    break;
                case SearchScopeVideo.Category:
                    query = query.Where(v => EF.Functions.Like(v.Category.Name, search));
                    break;

                case SearchScopeVideo.Uncategorised:
                    query = query.Where(v => v.Category == null && EF.Functions.Like(v.Title, $"%{search}%"));
                    break;

                case SearchScopeVideo.Tags:
                    query = query.Where(v => v.Tags.Any(t => EF.Functions.Like(t.Value, $"%{search}%")));
                    break;

                case SearchScopeVideo.All:
                default:
                    query = query.Where(v =>
                                            EF.Functions.Like(v.Title, $"%{search}%") ||
                                            EF.Functions.Like(v.Description, $"%{search}%") ||
                                            v.Tags.Any(x => EF.Functions.Like(x.Value, $"%{search}%")) ||
                                            EF.Functions.Like(v.Category.Name, $"%{search}%"));
                    break;
            }
            query = Sort(query, sortOption);


            return await PaginateAsync<Video>(query, skip, take);
        }

        public async Task<(IEnumerable<Video> videos, int totalCount)> GetRecommendation(int skip, int take, Video reference)
        {
            var referenceTagIds = reference.Tags.Select(t => t.Value).ToList();
            var referenceCategoryId = reference.CategoryId;
            int categoryWeight = 5;
            int tagsWeight = 8;

            var query = _dbSet
                    .AsNoTracking()
                    .Include(v => v.Category)
                    .Include(v => v.Tags)
                    .Where(v => v.Category != null)
                    .Where(v => v.Id != reference.Id)
                    .Select(v => new
                    {
                        Video = v,
                        CommonTagCount = v.Tags.Count(t => referenceTagIds.Contains(t.Value)),
                        SameCategory = v.CategoryId == referenceCategoryId
                    })
                    .Select(x => new
                    {
                        x.Video,
                        Score = (x.CommonTagCount * tagsWeight) + (x.SameCategory ? categoryWeight : 0)
                    })
                    .OrderByDescending(x => x.Score);

            return await PaginateAsync<Video>(query.Select(x => x.Video), skip, take);
        }

        public async Task InsertRangeAsync(IEnumerable<Video> batch)
        {
            await _dbSet.AddRangeAsync(batch);
        }

        public new async Task<Video?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(v => v.Tags)
                .Include(v => v.Category).SingleOrDefaultAsync(v => v.Id == id);
        }

        private IQueryable<Video> Sort( IQueryable<Video> query, SortOption<SortVideo> sortOption)
        {
            if (sortOption != null)
            {
                query = (sortOption.SortBy, sortOption.Direction) switch
                {
                    (SortVideo.Title, SortDirection.Ascending) => query.OrderBy(v => v.Title),
                    (SortVideo.Title, SortDirection.Descending) => query.OrderByDescending(v => v.Title),
                    (SortVideo.Update, SortDirection.Ascending) => query.OrderBy(v => v.Updated),
                    (SortVideo.Update, SortDirection.Descending) => query.OrderByDescending(v => v.Updated),
                    _ => query.OrderBy(v => v.Updated),
                };
            }
            else
            {
                query = query.OrderBy(v => v.Updated);
            }
            return query;
        }
    }
}
