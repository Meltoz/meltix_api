using Application.DTOs;
using Application.Interfaces.Repository;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace Meltix.UnitTests.Application
{
    public class UserServiceTests
    {
        [Fact]
        public async Task GetAll_ShouldReturnUser()
        {
            // Arrange
            var user = new User("Meltoz", "P@ssw0rd", Role.Administrator);
            var repoMock = new Mock<IUserRepository>();
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User> { user });

            var mapper = MapperFactory.Create();
            var service = new UserService(repoMock.Object, mapper);

            // Act
            var users = await service.GetAllAsync();

            // Assert
            Assert.NotNull(users);
            Assert.IsAssignableFrom<IEnumerable<UserDTO>>(users);
            Assert.Single(users);
        }
    }
}
