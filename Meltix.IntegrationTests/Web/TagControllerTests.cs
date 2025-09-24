using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Text.Json;
using Web.ViewModels;

namespace Meltix.IntegrationTests.Web
{
    public class TagControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public TagControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Search_ShouldReturnTag_WhenTagExist()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();

            var tagId = Guid.NewGuid();
            var tagName = "boule";
            var tag = new Tag(tagName) { Id=tagId};
            context.Tags.Add(tag);
            await context.SaveChangesAsync();

            var factory = new CustomWebApplicationFactory(context);
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/tag/Search?pageIndex=0&pageSize=10");
            response.EnsureSuccessStatusCode();

            // Assert
            var tags = JsonSerializer.Deserialize<List<TagVM>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.Single(tags);
            Assert.Equal(tagName, tags[0].Name);
            Assert.Equal(tagId, tags[0].Id);
        }

    }
}
