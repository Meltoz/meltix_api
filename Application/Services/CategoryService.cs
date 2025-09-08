using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Shared;
using Shared.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly CategoryRepository _categoryRepo;
        private readonly IMapper _mapper;

        public CategoryService(MeltixContext context, IMapper m)
        {
            _categoryRepo = new CategoryRepository(context);
            _mapper = m;
        }

        public async Task<ServiceResponse<CategoryDTO>> AddCategory(string categoryName)
        {
            var response = new ServiceResponse<CategoryDTO>();
            var c = new Category(categoryName);

            try
            {
                c = await _categoryRepo.InsertAsync(c);
                response.Response = _mapper.Map<CategoryDTO>(c);
                response.Status = ServiceResponseStatus.Success;
            }
            catch (InvalidOperationException ex)
            {
                response.Message = ex.Message;
                response.Status = ServiceResponseStatus.Failure;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = ServiceResponseStatus.Failure;
            }

            return response;
        }

        public ServiceResponse<bool> DeleteCategory(Guid idCategory)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                _categoryRepo.Delete(idCategory);
                response.Response = true;
                response.Status = ServiceResponseStatus.Success;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = ServiceResponseStatus.Failure;
            }

            return response;
        }

        public async Task<ServiceResponse<CategoryDTO>> GetByIdAsync(Guid id)
        {
            var response = new ServiceResponse<CategoryDTO>();
            try
            {
                var category = await _categoryRepo.GetByIdAsync(id);
                response.Response = _mapper.Map<CategoryDTO>(category);
                response.Status = ServiceResponseStatus.Success;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = ServiceResponseStatus.Failure;
            }
            return response;
        }

        public async Task<ServiceResponse<CategoryDTO>> GetByNameAsync(string name)
        {
            var response = new ServiceResponse<CategoryDTO>();

            try
            {
                var category = await _categoryRepo.ByNameAsync(name);
                response.Response = _mapper.Map<CategoryDTO>(category);
                response.Status = ServiceResponseStatus.Success;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = ServiceResponseStatus.Failure;
            }
            return response;
        }

        public async Task<ServiceResponse<(IEnumerable<CategoryDTO> categories, int totalCount)>> Search(int pageIndex, int pageSize, string categoryName)
        {
            var skip = pageIndex < 0 ? 0 : pageIndex * pageSize;

            try
            {
                var r = await _categoryRepo.Search(skip, pageSize, categoryName);
                var categories = _mapper.Map<IEnumerable<CategoryDTO>>(r.categories);
                return ServiceResponse<(IEnumerable<CategoryDTO> categories, int totalcount)>.Success((categories, r.totalCount));
            }
            catch (Exception ex)
            {
                return ServiceResponse<(IEnumerable<CategoryDTO> categories, int totalcount)>.Failure(ex.Message);
            }
        }

        public async Task<CategoryDTO> UpdateCategory(CategoryDTO categoryToUpdate)
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
