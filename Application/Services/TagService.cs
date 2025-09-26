using Application.DTOs;
using Application.Interfaces.Repository;
using AutoMapper;
using Shared;
using Shared.Exceptions;

namespace Application.Services
{
    public class TagService(ITagRepository tr, IMapper m)
    {
        private readonly ITagRepository _tagRepository = tr;
        private readonly IMapper _mapper = m;

        public async Task<PagedResult<TagDTO>> Search(int pageIndex, int pageSize, string searchTerm)
        {
            var skip = SkipCalculator.Calculate(pageIndex, pageSize);

            var tags =  await _tagRepository.Search(skip, pageSize, searchTerm);

            var tagsDto = _mapper.Map<IEnumerable<TagDTO>>(tags.tags);

            return new PagedResult<TagDTO>
            {
                Data = tagsDto,
                TotalCount = tags.totalCount
            };            
        }

        public async Task<TagDTO> Edit(Guid id, string value)
        {
            var tagToUpdate = await _tagRepository.GetByIdAsync(id);

            if (tagToUpdate is null)
                throw new EntityNotFoundException($"Impossible to find tag with id = '{id}'");

            tagToUpdate.ChangeValue(value);

            var tagUpdated = await _tagRepository.UpdateAsync(tagToUpdate);

            return _mapper.Map<TagDTO>(tagUpdated);

        }

        public async Task<bool> DeleteTag(Guid id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);

            if(tag is null)
                throw new EntityNotFoundException($"Impossible to find tag with id = '{id}'");

            _tagRepository.Delete(id);

            return true;
        }


    }
}
