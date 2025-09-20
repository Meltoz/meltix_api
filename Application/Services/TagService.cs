using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Shared;

namespace Application.Services
{
    public class TagService : ITagService
    {
        private readonly TagRepository _tagRepository;

        public TagService(MeltixContext ctx)
        {
            _tagRepository = new TagRepository(ctx);
        }
        public async Task<PagedResult<Tuple<string, int>>> Search(int pageIndex, int pageSize, string searchTerm)
        {
            var skip = SkipCalculator.Calculate(pageIndex, pageSize);

            var tags =  await _tagRepository.Search(skip, pageSize, searchTerm);

            return new PagedResult<Tuple<string, int>>
            {
                Data = tags.tags,
                TotalCount = tags.totalCount,
            };            
        }
    }
}
