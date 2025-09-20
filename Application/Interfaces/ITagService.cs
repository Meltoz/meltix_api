using Shared;

namespace Application.Interfaces
{
    public interface ITagService
    {
        public Task<PagedResult<Tuple<string, int>>> Search(int pageIndex, int pageSize, string searchTerm);
    }
}
