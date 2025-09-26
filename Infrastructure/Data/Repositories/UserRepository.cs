using Application.Interfaces.Repository;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Enums.Sorting;
using Shared.Enums.Sorting.User;

namespace Infrastructure.Data.Repositories
{
    public class UserRepository(MeltixContext context) : GenericRepository<User>(context), IUserRepository
    {
        public async Task<PagedResult<User>> Search(int skip, int take, SortOption<SortUser> sortOption, bool onlyAdmin, string? search)
        {
            var query = _dbSet
                .AsNoTracking()
                .AsQueryable();

            if(onlyAdmin)
            {
                query = query.Where(u => u.Role == Role.Administrator);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => u.Pseudo.Value.Contains(search));
            }

            query = SortQuery(query, sortOption);

           return await PaginateAsync<User>(query, skip, take);

        }

        private IQueryable<User> SortQuery(IQueryable<User> query, SortOption<SortUser> sortOption)
        {
            if(query != null)
            {
                query = (sortOption.SortBy, sortOption.Direction) switch
                {
                    (SortUser.Create, SortDirection.Ascending) => query.OrderBy(u => u.Created),
                    (SortUser.Create, SortDirection.Descending) => query.OrderByDescending(u => u.Created),
                    (SortUser.Role, SortDirection.Ascending) => query.OrderBy(u => u.Role),
                    (SortUser.Role, SortDirection.Descending) => query.OrderByDescending(u => u.Role),
                    (SortUser.Pseudo, SortDirection.Ascending) => query.OrderBy(u => u.Pseudo.Value),
                    (SortUser.Pseudo, SortDirection.Descending) => query.OrderByDescending(u => u.Pseudo.Value),
                    (SortUser.LastLoggin, SortDirection.Ascending) => query.OrderBy(u => u.LastLogin),
                    (SortUser.LastLoggin, SortDirection.Descending) => query.OrderByDescending(u => u.LastLogin),
                    _ => query.OrderBy(u => u.Created)
                };
            }
            else
            {
                query = query.OrderBy(u => u.Created);
            }

            return query;
        }
    }
}
