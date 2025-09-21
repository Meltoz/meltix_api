using Application.Interfaces;
using AutoMapper;
using meltix_web.Constantes;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly IMapper _mapper;

        public TagController(ITagService ts, IMapper m): base()
        {
            _tagService = ts;
            _mapper = m;
        }

        [HttpGet]
        public async Task<IActionResult> Search(int pageIndex, int pageSize, string? searchTerm)
        {
           searchTerm = searchTerm ?? string.Empty;

            var tags = await _tagService.Search(pageIndex, pageSize, searchTerm.ToLower());

            Response.Headers.Append(ApiConstantes.HeaderTotalCount, tags.TotalCount.ToString());

            var tagsReturned = _mapper.Map<IEnumerable<TagVM>>(tags.Data);
            return Ok(tagsReturned);
        }

        [HttpPatch]
        public async Task<IActionResult> Edit(TagVM tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var tagUpdate = await _tagService.Edit(tag.Id, tag.Name);

            return Ok(_mapper.Map<TagVM>(tagUpdate));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _tagService.DeleteTag(id);

            return Ok();
        }
    }
}
