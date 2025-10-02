using Application.DTOs;
using Application.Interfaces.Repository;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
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

        public async Task<UserDTO> CreateUser(string pseudo, string password, RoleDTO role)
        {
            var userRole = _mapper.Map<Role>(role);
            var user = new User(pseudo, password, userRole);

            var userAdded = await _userRepository.InsertAsync(user);

            return _mapper.Map<UserDTO>(userAdded);
        }

        public async Task<UserDTO> EditUserAdmin(Guid id, string pseudo, string password, RoleDTO role)
        {
            var roleUser = _mapper.Map<Role>(role);
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new EntityNotFoundException($"User not found with id '{id}'");

            // Vérification Role
            if (roleUser != user.Role)
                user.ChangeRole(roleUser);

            if (!string.IsNullOrEmpty(password) && Password.FromPlainText(password) != user.Password)
                user.ChangePassword(password);

            if(pseudo != user.Pseudo)
                   user.ChangePseudo(pseudo);

            var userUpdated = await _userRepository.UpdateAsync(user);
            return _mapper.Map<UserDTO>(userUpdated);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
                throw new EntityNotFoundException("User not found");

            _userRepository.Delete(id);

            return true;
        }

        public async Task<UserDTO> ValidateUser(string pseudo, string password)
        {
            var testedPassword = Password.FromPlainText(password);
            var testedPseudo = Pseudo.Create(pseudo);

            var user = await _userRepository.AuthUser(testedPseudo, testedPassword);

            if (user is null)
                throw new EntityNotFoundException("User not found");

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
                throw new EntityNotFoundException($"user with id '{id}' not found");

            return _mapper.Map<UserDTO>(user);
        }
    }
}
