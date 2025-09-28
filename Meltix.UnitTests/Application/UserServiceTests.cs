using Application.DTOs;
using Application.Interfaces.Repository;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Moq;
using Shared;
using Shared.Enums.Sorting;
using Shared.Enums.Sorting.User;
using Shared.Exceptions;

namespace Meltix.UnitTests.Application
{
    public class UserServiceTests
    {
        private readonly string _initialPseudo = "Meltoz";
        private readonly string _initialPassword = "P@ssw0rd";
        private readonly RoleDTO _initialRoleUser = RoleDTO.User;
        private readonly RoleDTO _initialRoleAdmin = RoleDTO.Admin;

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

        [Fact]
        public async Task CreateUser_WhenProvideUser_ShouldCreateUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User(_initialPseudo, _initialPassword, Role.User) { Id= userId};
            var repoMock = new Mock<IUserRepository>();
            repoMock.Setup(r => r.InsertAsync(It.IsAny<User>())).ReturnsAsync(user);

            var mapper = MapperFactory.Create();
            var service = new UserService(repoMock.Object, mapper);

            // Act
            var userCreated = await service.CreateUser(_initialPseudo, _initialPassword, _initialRoleUser);

            // Assert
            repoMock.Verify(r => r.InsertAsync(It.IsAny<User>()), Times.Once);
            Assert.NotNull(userCreated);
            Assert.Equal(_initialPseudo, userCreated.Pseudo);
            Assert.Equal(_initialRoleUser, userCreated.Role);
        }

        [Fact]
        public async Task EditUserAdmin_WhenPseudoChanged_ShouldChangePseudo()
        {
            // Assert
            var userId = Guid.NewGuid();
            var newPseudo = "TestUser";
            var user = new User(_initialPseudo, _initialPassword, Role.User) { Id = userId };
            var newUser = new User(newPseudo, _initialPassword, Role.User) { Id = userId };
            var repoMock = new Mock<IUserRepository>();
            repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            repoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(newUser);

            var mapper = MapperFactory.Create();
            var service = new UserService(repoMock.Object, mapper);

            // Act
            var userUpdated = await service.EditUserAdmin(userId, newPseudo, null, RoleDTO.User);

            // Assert
            repoMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            repoMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
            Assert.NotNull(userUpdated);
            Assert.Equal(newPseudo, userUpdated.Pseudo);
        }

        [Fact]
        public async Task EditUserAdmin_WhenNoUser_ShouldThrowEntityNotFoundException()
        {
            // Assert
            var userId = Guid.NewGuid();
            var newPseudo = "TestUser";
            var repoMock = new Mock<IUserRepository>();
            repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);

            var mapper = MapperFactory.Create();
            var service = new UserService(repoMock.Object, mapper);

            // Act
            var caughtException = await Assert.ThrowsAnyAsync<Exception>(() => service.EditUserAdmin(userId, newPseudo, null, RoleDTO.User));

            // Assert
            repoMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            Assert.NotNull(caughtException);
            Assert.IsType<EntityNotFoundException>(caughtException);
        }

        [Fact]
        public async Task DeleteUser_ShouldDeleteUser_WhenUserExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User("Meltoz", "P@ssw0rd", Role.User) {Id = userId };
            var repoMock = new Mock<IUserRepository>();
            repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            repoMock.Setup(r => r.Delete(userId));

            var mapper = MapperFactory.Create();
            var service = new UserService(repoMock.Object, mapper);


            // Act
            var response = await service.DeleteUserAsync(userId);

            // Assert
            Assert.True(response);
            repoMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            repoMock.Verify(r => r.Delete(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ShouldThrow_WhenUserNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var repoMock = new Mock<IUserRepository>();
            repoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);

            var mapper = MapperFactory.Create();
            var service = new UserService(repoMock.Object, mapper);


            // Act
            var caughtException = await Assert.ThrowsAnyAsync<Exception>(() => service.DeleteUserAsync(userId));

            // Assert
            Assert.IsType<EntityNotFoundException>(caughtException);
            repoMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
        }
    }
}
