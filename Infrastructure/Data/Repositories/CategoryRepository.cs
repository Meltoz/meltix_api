using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class CategoryRepository : GenericRepository<Category>
    {
        public CategoryRepository(MeltixContext m) : base(m)
        {

        }

        public override async Task<Category?> InsertAsync(Category entity)
        {
            if (await _dbSet.AnyAsync(c => c.Name.ToLower() == entity.Name.ToLower()))
            {
                throw new InvalidOperationException("Same category already exist in db");
            }

            return await base.InsertAsync(entity);
        }

        public async Task<(IEnumerable<Category> categories, int totalCount)> Search(int skip, int take, string categoryName)
        {
            var query = _dbSet.Where(x => x.Name.ToLower().Contains(categoryName.ToLower()));

            var totalCount = await query.CountAsync();
            var categories = await query.Skip(skip).Take(take).ToListAsync();

            return (categories, totalCount);
        }
    }
}
