using Domain.Entities;
using Domain.ValueObjects;
using Shared;
using Shared.Enums.Sorting.User;

namespace Application.Interfaces.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<PagedResult<User>> Search(int skip, int take, SortOption<SortUser> sortOption, bool onlyAdmin, string search = "");

        public Task<User> AuthUser(Pseudo pseudo, Password password);
    }
}
