using Application.Interfaces.Repository;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class CategoryRepository(MeltixContext context) : GenericRepository<Category>(context), ICategoryRepository
    {
        public async Task<Category?> ByNameAsync(string name)
        {
            var category = await _dbSet.Where(c => c.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();

            return category;
        }

        /// <summary>
        /// Permet de rechercher une catégorie par son nom
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public async Task<(IEnumerable<Category> categories, int totalCount)> Search(int skip, int take, string categoryName)
        {
            var query = _dbSet.Where(x => x.Name.ToLower().Contains(categoryName.ToLower()));

            var totalCount = await query.CountAsync();
            var categories = await query.Skip(skip).Take(take).ToListAsync();

            return (categories, totalCount);
        }

        public new async Task<Category?> InsertAsync(Category entity)
        {
            if (await _dbSet.AnyAsync(c => c.Name.ToLower() == entity.Name.ToLower()))
            {
                throw new InvalidOperationException("Same category already exist in db");
            }

            return await base.InsertAsync(entity);
        }

        
    }
}
