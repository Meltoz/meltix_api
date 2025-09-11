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
    }
}
