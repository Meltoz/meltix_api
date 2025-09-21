using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using meltix_web.Constantes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageIndex, int pageSize, string? name)
        {
            if (pageIndex < 0 || pageSize < 1)
                return BadRequest();

            var categoriesDtos = await _categoryService.SearchAsync(pageIndex, pageSize, name ?? "");

            var categories = _mapper.Map<IEnumerable<CategoryVM>>(categoriesDtos.categories);

            Response.Headers.Append(ApiConstantes.HeaderTotalCount, categoriesDtos.totalCount.ToString());
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> StoreCategory([FromBody]CategoryVM category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryAdded = await _categoryService.AddCategoryAsync(category.Name);
           
            return Ok(_mapper.Map<CategoryVM>(categoryAdded));
        }


        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryVM category)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var dto = _mapper.Map<CategoryDTO>(category);

            var updatedCategory = await _categoryService.UpdateCategoryAsync(dto);

            return Ok(_mapper.Map<CategoryVM>(updatedCategory));
        }

        [HttpDelete]
        public IActionResult DeleteCategory(Guid id)
        {
            _categoryService.DeleteCategory(id);

            return Ok();
        }
    }
}
