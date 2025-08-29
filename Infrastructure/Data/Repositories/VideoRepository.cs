using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class VideoRepository : GenericRepository<Video>
    {
        public VideoRepository(MeltixContext context) : base(context)
        {
        }


        public async Task<(IEnumerable<Video> videos, int totalCount)> Search(int skip, int take, string search)
        {
            var searchLower = search.ToLower();

            var query = _dbSet.Include(x => x.Category)
                .Where(v => v.Title.ToLower().Contains(searchLower) || v.Description.ToLower().Contains(searchLower));

            var videos = await query.Skip(skip).Take(take).ToListAsync();
            var totalCount = await query.CountAsync();

            return (videos, totalCount);
        }
    }
}
