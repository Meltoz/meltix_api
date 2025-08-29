using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Web.ViewModels;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CategoryController : ControllerBase
    {

        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService cs, IMapper m)
        {
            _categoryService = cs;
            _mapper = m;
        }

        [HttpPost]
        public async Task<IActionResult> StoreCategory([FromBody] string name)
        {
            var response = await _categoryService.AddCategory(name);
            if (response.Status != ServiceResponseStatus.Success)
            {
                return StatusCode(500);
            }
            return Ok(response.Response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? name)
        {
            var responseSearch = await _categoryService.Search(0, 20, name ?? "");
            if (responseSearch.Status != ServiceResponseStatus.Success)
            {
                return StatusCode(500);
            }
            var categoriesDTO  = responseSearch.Response;
            IEnumerable<CategoryVM> categories;
            
            try
            {
                categories = _mapper.Map<IEnumerable<CategoryVM>>(categoriesDTO.categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500); 
            }
            return Ok(new { categories = categories, totalCount = categoriesDTO.totalCount });
        }
    }
}
