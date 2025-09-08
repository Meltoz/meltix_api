using Application.DTOs;
using Shared;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        public Task<(IEnumerable<CategoryDTO> categories, int totalCount)> SearchAsync(int pageIndex, int pageSize, string categoryName);

        public Task<CategoryDTO> AddCategoryAsync(string categoryName);

        public Task<CategoryDTO> UpdateCategoryAsync(CategoryDTO categoryToUpdate);

        public bool DeleteCategory(Guid idCategory);

        public Task<CategoryDTO> GetByIdAsync(Guid id);

        public Task<CategoryDTO> GetByNameAsync(string name);
    }
}
