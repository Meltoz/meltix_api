using Application.DTOs;
using Application.Interfaces.Repository;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Moq;
using Shared;
using Shared.Enums.Sorting;
using Shared.Enums.Sorting.User;

namespace Meltix.UnitTests.Application
{
    public class UserServiceTests
    {
        [Fact]
        public async Task Paginate_ShouldReturnUser()
        {
            // Arrange
            var user = new User("Meltoz", "P@ssw0rd", Role.Administrator);
            var repoMock = new Mock<IUserRepository>();
            repoMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortUser>>(),
                It.IsAny<bool>(),
                It.IsAny<string>()
                )).ReturnsAsync(new PagedResult<User> { Data = new List<User> { user }, TotalCount = 1 });
            var sortOption = new SortOption<SortUser>() { Direction = SortDirection.Ascending, SortBy = SortUser.Pseudo };

            var mapper = MapperFactory.Create();
            var service = new UserService(repoMock.Object, mapper);


            // Act
            var users = await service.PaginateAsync(0, 10, sortOption, false);

            // Assert
            Assert.NotNull(users);
            Assert.IsAssignableFrom<IEnumerable<UserDTO>>(users.Data);
            Assert.Single(users.Data);
        }
    }
}
