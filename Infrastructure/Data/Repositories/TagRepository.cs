using Application.Interfaces.Repository;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class TagRepository : GenericRepository<Tag>, ITagRepository
    {
        public TagRepository(MeltixContext context) : base(context)
        {
        
        }

        public async Task<Tag?> GetByNameAsync(string value)
        {
            return await _dbSet.SingleOrDefaultAsync(x => x.Value.ToLower() == value.ToLower());
        }

        public async Task<(IEnumerable<Tag> tags, int totalCount)> Search(int skip, int take, string search)
        {
            var query = _dbSet
                .Include(t => t.Videos)
                .Where(t => t.Value.Contains(search))
                .OrderBy(t => t.Value);

            var totalCount = await query.CountAsync();
            var tags = await query.Skip(skip).Take(take).ToListAsync();

            return (tags, totalCount);
                
         
        }
    }
}
