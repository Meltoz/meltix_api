using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class TagRepository : GenericRepository<Tag>
    {
        public TagRepository(MeltixContext context) : base(context)
        {
        
        }

        public async Task<Tag?> GetByNameAsync(string value)
        {
            return await _dbSet.SingleOrDefaultAsync(x => x.Value.ToLower() == value.ToLower());
        }

        public async Task<IEnumerable<Tag>> Search(int limit, string search)
        {
            return await _dbSet.Where(t => t.Value.Contains(search))
                .OrderBy(t => t.Value)
                .Take(limit)
                .ToListAsync();
        }
    }
}
