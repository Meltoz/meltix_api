using Application.DTOs;
using Shared;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        public Task<ServiceResponse<(IEnumerable<CategoryDTO> categories, int totalCount)>> Search(int pageIndex, int pageSize, string categoryName);

        public Task<ServiceResponse<CategoryDTO>> AddCategory(string categoryName);

        public Task<ServiceResponse<CategoryDTO>> UpdateCategory(CategoryDTO categoryToUpdate);

        public ServiceResponse<bool> DeleteCategory(Guid idCategory);

        public Task<ServiceResponse<CategoryDTO>> GetByIdAsync(Guid id);

        public Task<ServiceResponse<CategoryDTO>> GetByNameAsync(string name);
    }
}
