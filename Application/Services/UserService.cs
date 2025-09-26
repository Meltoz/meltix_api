using Application.DTOs;
using Application.Interfaces.Repository;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Shared;
using Shared.Enums.Sorting.User;
using Shared.Exceptions;

namespace Application.Services
{
    public class UserService (IUserRepository ur, IMapper m)
    {
        private readonly IUserRepository _userRepository =ur;
        private readonly IMapper _mapper = m;

        public async Task<PagedResult<UserDTO>> PaginateAsync(int pageIndex, int pageSize, SortOption<SortUser> sort, bool onlyAdmin, string search= "")
        {
            var skip = SkipCalculator.Calculate(pageIndex, pageSize);

            var response = await _userRepository.Search(skip, pageSize, sort, onlyAdmin, search);

            var users = _mapper.Map<IEnumerable<UserDTO>>(response.Data);

            return new PagedResult<UserDTO> {
                Data = users,
                TotalCount = response.TotalCount

            };
        }

        public async Task<UserDTO> CreateUser(string pseudo, string password)
        {
            var user = new User(pseudo, password, Role.User);

            var userAdded = await _userRepository.InsertAsync(user);

            return _mapper.Map<UserDTO>(userAdded);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
                throw new EntityNotFoundException("User not found");

            _userRepository.Delete(id);

            return true;
        }
    }
}
