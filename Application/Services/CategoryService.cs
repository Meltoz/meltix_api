using Application.DTOs;
using Application.Interfaces.Repository;
using AutoMapper;
using Domain.Entities;
using Shared.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Application.Services
{
    public class CategoryService(ICategoryRepository c, IMapper m)
    {
        private readonly ICategoryRepository _categoryRepo = c;
        private readonly IMapper _mapper =m;

        public async Task<CategoryDTO> AddCategoryAsync(string categoryName)
        {
            var c = new Category(categoryName);

            if (string.IsNullOrEmpty(categoryName.Trim()))
                throw new ValidationException($"Category name not to be empty");

            var category = await _categoryRepo.InsertAsync(c);

            return _mapper.Map<CategoryDTO>(category);
        }

        public bool DeleteCategory(Guid idCategory)
        {
            var categoryToDelete = _categoryRepo.GetById(idCategory);

            if (categoryToDelete is null)
                throw new EntityNotFoundException($"Category not found with id {idCategory}");


            _categoryRepo.Delete(idCategory);

            return true;
        }

        public async Task<CategoryDTO> GetByIdAsync(Guid id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category is null)
                throw new EntityNotFoundException($"Category with id '{id}' doesnt exist");

            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<CategoryDTO> GetByNameAsync(string name)
        {
            var category = await _categoryRepo.ByNameAsync(name);
            if (category is null)
                throw new EntityNotFoundException($"Category with name '{name}' doesnt exists");

            return _mapper.Map<CategoryDTO>(category);

        }

        public async Task<(IEnumerable<CategoryDTO> categories, int totalCount)> SearchAsync(int pageIndex, int pageSize, string categoryName)
        {
            var skip = pageIndex < 0 ? 0 : pageIndex * pageSize;

            var r = await _categoryRepo.Search(skip, pageSize, categoryName);
            var categories = _mapper.Map<IEnumerable<CategoryDTO>>(r.categories);

            return (categories, r.totalCount);
        }

        public async Task<CategoryDTO> UpdateCategoryAsync(CategoryDTO categoryToUpdate)
        {
            var category = await _categoryRepo.GetByIdAsync(categoryToUpdate.Id);

            if (category == null)
            {
                throw new EntityNotFoundException($"Category with {categoryToUpdate.Id} not found");
            }

            if (string.IsNullOrWhiteSpace(categoryToUpdate.Name))
                throw new ValidationException("Category name cannot be empty.");

            category.ChangeName(categoryToUpdate.Name);

            var categoryUpdate = await _categoryRepo.UpdateAsync(category);

            return _mapper.Map<CategoryDTO>(categoryUpdate);
        }
    }
}
