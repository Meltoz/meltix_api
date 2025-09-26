using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Web.ViewModels;

namespace Meltix.IntegrationTests.Web
{
    public class UserControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public UserControllerTests(WebApplicationFactory<Program> f)
        {
            _factory = f;
        }

        [Fact]
        public async Task Search_ShouldReturnListUser_WhenUserExist()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var user1 = new User("Meltoz", "P@ssw0rd", Role.Administrator);
            var user2 = new User("Mayloutre", "P@ssw0rd", Role.User);
            await context.Users.AddRangeAsync([user1, user2]);
            await context.SaveChangesAsync();

            var factory = new CustomWebApplicationFactory(context);
            var client = factory.CreateClient();
            var param = "pageIndex=0&pageSize=10&sort=create_ascending&onlyAdmin=false";

            // Act
            var response = await client.GetAsync($"/api/user/search?{param}");
            response.EnsureSuccessStatusCode();

            // Assert
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public async Task Search_ShouldReturnOnlyAdmin_WhenUserExist()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var user1 = new User("Meltoz", "P@ssw0rd", Role.Administrator);
            var user2 = new User("Mayloutre", "P@ssw0rd", Role.User);
            await context.Users.AddRangeAsync([user1, user2]);
            await context.SaveChangesAsync();

            var factory = new CustomWebApplicationFactory(context);
            var client = factory.CreateClient();
            var param = "pageIndex=0&pageSize=10&sort=create_ascending&onlyAdmin=true";

            // Act
            var response = await client.GetAsync($"/api/user/search?{param}");
            response.EnsureSuccessStatusCode();

            // Assert
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(users);
            Assert.Single(users);
        }

        [Fact]
        public async Task Search_ShouldReturnAlphabetical_WhenSortedByPseudoAscending()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var user1 = new User("Meltoz", "P@ssw0rd", Role.Administrator);
            var user2 = new User("Mayloutre", "P@ssw0rd", Role.User);
            await context.Users.AddRangeAsync([user1, user2]);
            await context.SaveChangesAsync();

            var factory = new CustomWebApplicationFactory(context);
            var client = factory.CreateClient();
            var param = "pageIndex=0&pageSize=10&sort=pseudo_ascending&onlyAdmin=false";

            // Act
            var response = await client.GetAsync($"/api/user/search?{param}");
            response.EnsureSuccessStatusCode();

            // Assert
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
            Assert.Equal("Mayloutre", users[0].Pseudo);
        }

        [Fact]
        public async Task Search_ShouldReturnNonAlphabetical_WhenSortedByPseudoDescending()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var user1 = new User("Meltoz", "P@ssw0rd", Role.Administrator);
            var user2 = new User("Mayloutre", "P@ssw0rd", Role.User);
            await context.Users.AddRangeAsync([user1, user2]);
            await context.SaveChangesAsync();

            var factory = new CustomWebApplicationFactory(context);
            var client = factory.CreateClient();
            var param = "pageIndex=0&pageSize=10&sort=pseudo_descending&onlyAdmin=false";

            // Act
            var response = await client.GetAsync($"/api/user/search?{param}");
            response.EnsureSuccessStatusCode();

            // Assert
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
            Assert.Equal("Meltoz", users[0].Pseudo);
        }

        [Fact]
        public async Task Search_ShouldReturnAdminFirst_WhenSortedByRoleAscending()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var user1 = new User("Meltoz", "P@ssw0rd", Role.Administrator);
            var user2 = new User("Mayloutre", "P@ssw0rd", Role.User);
            await context.Users.AddRangeAsync([user1, user2]);
            await context.SaveChangesAsync();

            var factory = new CustomWebApplicationFactory(context);
            var client = factory.CreateClient();
            var param = "pageIndex=0&pageSize=10&sort=role_ascending&onlyAdmin=false";

            // Act
            var response = await client.GetAsync($"/api/user/search?{param}");
            response.EnsureSuccessStatusCode();

            // Assert
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
            Assert.Equal("Meltoz", users[0].Pseudo);
            Assert.Equal("Mayloutre", users[1].Pseudo);
        }

        [Fact]
        public async Task Search_ShouldReturnUserFirst_WhenSortedByRoleDescending()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var user1 = new User("Meltoz", "P@ssw0rd", Role.Administrator);
            var user2 = new User("Mayloutre", "P@ssw0rd", Role.User);
            await context.Users.AddRangeAsync([user1, user2]);
            await context.SaveChangesAsync();

            var factory = new CustomWebApplicationFactory(context);
            var client = factory.CreateClient();
            var param = "pageIndex=0&pageSize=10&sort=role_descending&onlyAdmin=false";

            // Act
            var response = await client.GetAsync($"/api/user/search?{param}");
            response.EnsureSuccessStatusCode();

            // Assert
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
            Assert.Equal("Mayloutre", users[0].Pseudo);
            Assert.Equal("Meltoz", users[1].Pseudo);
        }

        [Fact]
        public async Task Search_ShouldReturn400_WhenSortDontMatch()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();

            var factory = new CustomWebApplicationFactory(context);
            var client = factory.CreateClient();
            var param = "pageIndex=0&pageSize=10&sort=title&onlyAdmin=false";

            // Act
            var response = await client.GetAsync($"/api/user/search?{param}");


            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Search_ShouldReturn400_WhenPageSizeLessThen1()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();

            var factory = new CustomWebApplicationFactory(context);
            var client = factory.CreateClient();
            var param = "pageIndex=0&pageSize=0&sort=pseudo_ascending&onlyAdmin=false";

            // Act
            var response = await client.GetAsync($"/api/user/search?{param}");


            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldCreateUser_WhenCorrect()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var factory = new CustomWebApplicationFactory(context);
            var client = factory.CreateClient();
            var user = new LoginRequestVM { Pseudo = "Meltoz", Password = "P@ssw0rd" };
            var content = JsonContent.Create(user);

            // Act
            var response = await client.PostAsync("/api/user/create", content);


            // Assert
            response.EnsureSuccessStatusCode();
            var userInDb = await context.Users.Where(x => x.Pseudo.Value == "Meltoz").SingleOrDefaultAsync();
            Assert.NotNull(userInDb);
        }

        [Fact]
        public async Task Create_ShouldReturnStatus400_WhenWeakPassword()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var factory = new CustomWebApplicationFactory(context);
            var client = factory.CreateClient();
            var user = new LoginRequestVM { Pseudo = "Meltoz", Password = "password" };
            var content = JsonContent.Create(user);

            // Act
            var response = await client.PostAsync("/api/user/create", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldReturnStatus400_WhenShortPseudo()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var factory = new CustomWebApplicationFactory(context);
            var client = factory.CreateClient();
            var user = new LoginRequestVM { Pseudo = "me", Password = "P@ssw0rd" };
            var content = JsonContent.Create(user);

            // Act
            var response = await client.PostAsync("/api/user/create", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldReturnStatus400_WhenLongPseudo()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var factory = new CustomWebApplicationFactory(context);
            var client = factory.CreateClient();
            var user = new LoginRequestVM { Pseudo = "azertyuiopqsdfghjklmwxcvbnazertyuiopqsdfghjklmwxcvbn", Password = "P@ssw0rd" };
            var content = JsonContent.Create(user);

            // Act
            var response = await client.PostAsync("/api/user/create", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldDeleteUser_WhenUserExist()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var userId = Guid.NewGuid();
            var user1 = new User("Meltoz", "P@ssw0rd", Role.Administrator) { Id= userId };
            var user2 = new User("Mayloutre", "P@ssw0rd", Role.User);
            await context.Users.AddRangeAsync([user1, user2]);
            await context.SaveChangesAsync();

            var factory = new CustomWebApplicationFactory(context);
            var client = factory.CreateClient();

            // Act
            var response = await client.DeleteAsync($"/api/user/delete?id={userId}");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Delete_ShouldReturn404_WhenUserNotExist()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var userId = Guid.NewGuid();

            var factory = new CustomWebApplicationFactory(context);
            var client = factory.CreateClient();

            // Act
            var response = await client.DeleteAsync($"/api/user/delete?id={userId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
