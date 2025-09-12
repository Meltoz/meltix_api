using Application.Interfaces;
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
        public async Task<IActionResult> Search(string searchTerm)
        {
            var tags = await _tagService.Search(10, searchTerm.ToLower());

            return Ok(tags);
        }
     
    }
}
