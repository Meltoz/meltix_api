using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;

namespace Application.Services
{
    public class TagService : ITagService
    {
        private readonly TagRepository _tagRepository;

        public TagService(MeltixContext ctx)
        {
            _tagRepository = new TagRepository(ctx);
        }
        public async Task<IEnumerable<string>> Search(int limit, string searchTerm)
        {
            var tags =  await _tagRepository.Search(limit, searchTerm);

            return tags.Select(t => t.Value);
        }
    }
}
