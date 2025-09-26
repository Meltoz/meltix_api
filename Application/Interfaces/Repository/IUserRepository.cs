using Domain.Entities;
using Shared;
using Shared.Enums.Sorting.User;

namespace Application.Interfaces.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<(IEnumerable<User> users, int totalCount)> Search(int skip, int take, SortOption<SortUser> sortOption, bool onlyAdmin, string? search);
    }
}
