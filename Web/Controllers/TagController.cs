using Application.Interfaces;
using meltix_web.Constantes;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagController(ITagService ts): base()
        {
            _tagService = ts;
        }

        [HttpGet]
        public async Task<IActionResult> Search(int pageIndex, int pageSize, string? searchTerm)
        {
           searchTerm = searchTerm ?? string.Empty;

            var tags = await _tagService.Search(pageIndex, pageSize, searchTerm.ToLower());

            Response.Headers.Append(ApiConstantes.HeaderTotalCount, tags.TotalCount.ToString());
            return Ok(tags.Data.Select(x => new {name= x.Item1, count= x.Item2}));
        }
     
    }
}
