using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        public Task<Category?> ByNameAsync(string name);

        public Task<(IEnumerable<Category> categories, int totalCount)> Search(int skip, int take, string categoryName);

        public new Task<Category?> InsertAsync(Category entity);
    }
}
