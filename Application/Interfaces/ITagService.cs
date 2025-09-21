using Application.DTOs;
using Shared;

namespace Application.Interfaces
{
    public interface ITagService
    {
        public Task<PagedResult<TagDTO>> Search(int pageIndex, int pageSize, string searchTerm);

        public Task<TagDTO> Edit(Guid id, string value);

        public Task<bool> DeleteTag(Guid id);
    }
}
