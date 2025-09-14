using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class VideoRepository : GenericRepository<Video>
    {
        public VideoRepository(MeltixContext context) : base(context)
        {
        }

        public async Task<Video?> GetBySlug(string slug)
        {
            return await _dbSet
                .Include(v => v.Category)
                .Include(v => v.Tags)
                .Where(v => v.Slug == slug).FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<Video> videos, int totalCount)> Search(int skip, int take, string search)
        {
            var searchLower = search.ToLower();

            var query = _dbSet
                .Include(v => v.Tags)
                .Include(x => x.Category)
                .Where(v => v.Title.ToLower().Contains(searchLower) || v.Description.ToLower().Contains(searchLower) 
                || v.Tags.Select(x =>x.Value).Any(x => x.Contains(searchLower))
                || (v.Category != null && v.Category.Name.ToLower().Contains(searchLower)));

            var videos = await query.Skip(skip).Take(take).ToListAsync();
            var totalCount = await query.CountAsync();

            return (videos, totalCount);
        }

        public async Task<(IEnumerable<Video> videos, int totalCount)> GetRecommendation(int skip, int take, Video reference)
        {
            var referenceTagIds = reference.Tags.Select(t => t.Value).ToList();
            var referenceCategoryId = reference.CategoryId;
            int categoryWeight = 5;
            int tagsWeight = 8;

            var query = _dbSet
                    .Include(v => v.Category)
                    .Include(v => v.Tags)
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

            var t = await query.Skip(0).Take(10).ToListAsync();

            var total = await query.CountAsync();
            var videos = await query
                .Select(x=> x.Video)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return (videos, total);
        }

        public async Task InsertRangeAsync(Video[] batch)
        {
            await _dbSet.AddRangeAsync(batch);
        }

        public override async Task<Video?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(v => v.Tags)
                .Include(v => v.Category).SingleOrDefaultAsync(v => v.Id == id);
        }


    }
}
