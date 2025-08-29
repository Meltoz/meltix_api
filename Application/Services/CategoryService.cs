using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Shared;

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

        public async Task<ServiceResponse<(IEnumerable<CategoryDTO> categories, int totalCount)>> Search(int pageIndex, int pageSize, string categoryName)
        {
            var response = new ServiceResponse<(IEnumerable<CategoryDTO> categories, int totalcount)>();
            var skip = pageIndex < 0 ? 0 : pageIndex * pageSize;

            try
            {
                var r = await _categoryRepo.Search(skip, pageSize, categoryName);
                var categories = _mapper.Map<IEnumerable<CategoryDTO>>(r.categories);
                response.Response = (categories, r.totalCount);
                response.Status = ServiceResponseStatus.Success;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = ServiceResponseStatus.Failure;
            }

            return response;

        }

        public Task<ServiceResponse<CategoryDTO>> UpdateCategory(CategoryDTO categoryToUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
