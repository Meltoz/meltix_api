using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface ITagRepository : IRepository<Tag>
    {
        public Task<Tag?> GetByNameAsync(string value);

        public Task<(IEnumerable<Tag> tags, int totalCount)> Search(int skip, int take, string search);
    }
}
