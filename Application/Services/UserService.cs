using Application.DTOs;
using Application.Interfaces.Repository;
using AutoMapper;
using Shared;
using Shared.Enums.Sorting.User;

namespace Application.Services
{
    public class UserService (IUserRepository ur, IMapper m)
    {
        private readonly IUserRepository _userRepository =ur;
        private readonly IMapper _mapper = m;

        public async Task<PagedResult<UserDTO>> PaginateAsync(int pageIndex, int pageSize, SortOption<SortUser> sort, bool onlyAdmin, string? search)
        {
            var skip = SkipCalculator.Calculate(pageIndex, pageSize);

            var response = await _userRepository.Search(skip, pageSize, sort, onlyAdmin, search);

            var users = _mapper.Map<IEnumerable<UserDTO>>(response.Data);

            return new PagedResult<UserDTO> {
                Data = users,
                TotalCount = response.TotalCount
            };
        }
    }
}
