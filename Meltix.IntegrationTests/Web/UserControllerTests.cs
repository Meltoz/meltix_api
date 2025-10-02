using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
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
        private readonly string _initialUserName = "Meltoz";
        private readonly string _intialPseudoUser2 = "Mayloutre";
        private readonly string _initialPassword= "P@ssw0rd";
        private readonly Role _initialRoleUser = Role.User;
        private readonly Role _initialRoleAdmin = Role.Administrator;

        public UserControllerTests(WebApplicationFactory<Program> f)
        {
            _factory = f;
        }

        [Fact]
        public async Task Search_ShouldReturnListUser_WhenUserExist()
        {
            // Arrange
            var context = await CreateInitial2User();
            var client = CreateClient(context);

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
            var context = await CreateInitial2User();
            var client = CreateClient(context);

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
            var context = await CreateInitial2User();
            var client = CreateClient(context);

            var param = "pageIndex=0&pageSize=10&sort=pseudo_ascending&onlyAdmin=false";

            // Act
            var response = await client.GetAsync($"/api/user/search?{param}");


            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
            Assert.Equal(_intialPseudoUser2, users.First().Pseudo);
        }

        [Fact]
        public async Task Search_ShouldReturnNonAlphabetical_WhenSortedByPseudoDescending()
        {
            // Arrange
            var context = await CreateInitial2User();
            var client = CreateClient(context);

            var param = "pageIndex=0&pageSize=10&sort=pseudo_descending&onlyAdmin=false";

            // Act
            var response = await client.GetAsync($"/api/user/search?{param}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
            Assert.Equal(_initialUserName, users.First().Pseudo);
        }

        [Fact]
        public async Task Search_ShouldReturnAdminFirst_WhenSortedByRoleAscending()
        {
            // Arrange
            var context = await CreateInitial2User();
            var client = CreateClient(context);

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
            Assert.Equal(_initialUserName, users.First().Pseudo);
            Assert.Equal(_intialPseudoUser2, users.Last().Pseudo);
        }

        [Fact]
        public async Task Search_ShouldReturnUserFirst_WhenSortedByRoleDescending()
        {
            // Arrange
            var context = await CreateInitial2User();
            var client = CreateClient(context);

            var param = "pageIndex=0&pageSize=10&sort=role_descending&onlyAdmin=false";

            // Act
            var response = await client.GetAsync($"/api/user/search?{param}");


            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            List<UserAdminVM>? users = JsonSerializer.Deserialize<List<UserAdminVM>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
            Assert.Equal(_intialPseudoUser2, users.First().Pseudo);
            Assert.Equal(_initialUserName, users.Last().Pseudo);
        }

        [Fact]
        public async Task Search_ShouldReturn400_WhenSortDontMatch()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var client = CreateClient(context);

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
            var user = new UserCreationVM { Pseudo = "Meltoz", Password = "P@ssw0rd", Role = RoleDTO.User };
            var content = JsonContent.Create(user);

            // Act
            var response = await client.PostAsync("/api/user/create", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            context.ChangeTracker.Clear();
            var userInDb = await context.Users.Where(x => x.Pseudo.Value == _initialUserName).SingleOrDefaultAsync();
            Assert.NotNull(userInDb);
            Assert.Equal(Role.User, userInDb.Role);
        }

        [Fact]
        public async Task Create_ShouldCreateAdmin_WhenCorrect()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var client = CreateClient(context);

            var user = new UserCreationVM { Pseudo = _initialUserName, Password = _initialPassword, Role = RoleDTO.Admin };
            var content = JsonContent.Create(user);

            // Act
            var response = await client.PostAsync("/api/user/create", content);


            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var userInDb = await context.Users.Where(x => x.Pseudo.Value == _initialUserName).SingleOrDefaultAsync();
            Assert.NotNull(userInDb);
            Assert.Equal(Role.Administrator, userInDb.Role);
        }

        [Fact]
        public async Task Create_ShouldReturnStatus400_WhenWeakPassword()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var client = CreateClient(context);
            var user = new UserCreationVM { Pseudo = _initialUserName, Password = "password" };
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
            var client = CreateClient(context);

            var user = new UserCreationVM { Pseudo = "me", Password = _initialPassword};
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
            var user = new UserCreationVM { Pseudo = "azertyuiopqsdfghjklmwxcvbnazertyuiopqsdfghjklmwxcvbn", Password = "P@ssw0rd" };
            var content = JsonContent.Create(user);

            // Act
            var response = await client.PostAsync("/api/user/create", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Create_ShouldEditUser_WhenCorrect()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var context = await CreateInitialUser(userId);
            var client = CreateClient(context);
            var newPseudo = "Meltoz2";

            var userToEdit = new UserCreationVM { Id = userId, Pseudo = newPseudo, Role = RoleDTO.Admin };
            var content = JsonContent.Create(userToEdit);

            // Act
            var response = await client.PatchAsync("/api/user/update", content);


            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var userInDb = await context.Users.Where(x => x.Pseudo.Value == newPseudo).SingleOrDefaultAsync();
            Assert.NotNull(userInDb);
            Assert.Equal(Role.Administrator, userInDb.Role);
        }

        [Fact]
        public async Task Create_ShouldRoleChanged_WhenRoleChange()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var context = await CreateInitialUser(userId);

            var client = CreateClient(context);
            var userToEdit = new UserCreationVM { Id = userId, Pseudo = _initialUserName, Role = RoleDTO.User };
            var content = JsonContent.Create(userToEdit);

            // Act
            var response = await client.PatchAsync("/api/user/update", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var userUpdated = JsonSerializer.Deserialize<UserAdminVM>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.NotNull(userUpdated);
            Assert.Equal("User", userUpdated.Role);

            context.ChangeTracker.Clear();
            var userInDb = await context.Users.Where(u => u.Pseudo.Value == _initialUserName).SingleOrDefaultAsync();
            Assert.NotNull(userInDb);
            Assert.Equal("User", userInDb.Role.ToString());
        }

        [Fact]
        public async Task Delete_WhenUserExist_ShouldDeleteUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var context = await CreateInitialUser(userId);
            var client = CreateClient(context);

            // Act
            var response = await client.DeleteAsync($"/api/user/delete?id={userId}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            context.ChangeTracker.Clear();
            var deletedUser = await context.Users.FindAsync(userId);
            Assert.Null(deletedUser);
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

        private HttpClient CreateClient(MeltixContext context)
        {
            var factory = new CustomWebApplicationFactory(context);
            return factory.CreateClient();
        }

        private async Task<MeltixContext> CreateInitialUser(Guid id)
        {
            var context = DbContextProvider.SetupContext();
            var userContext = new User(_initialUserName, _initialPassword, _initialRoleAdmin) { Id = id };
            context.Users.Add(userContext);
            await context.SaveChangesAsync();

            return context;
        }

        private async Task<MeltixContext> CreateInitial2User()
        {
            var context = DbContextProvider.SetupContext();
            var userContext = new User(_initialUserName, _initialPassword, _initialRoleAdmin);
            var user2 = new User(_intialPseudoUser2, _initialPassword, _initialRoleUser);
            context.Users.AddRange([userContext, user2]);
            await context.SaveChangesAsync();

            return context;
        }
    }
}
