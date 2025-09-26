using Application.Interfaces.Repository;
using Application.Services;
using Domain.Entities;
using Moq;
using Shared.Exceptions;

namespace Meltix.UnitTests.Application
{
    public class TagServiceTests
    {
        [Fact]
        public async Task DeleteTag_ShouldDeleteTag_WhenTagExist()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            var tag = new Tag("Test") { Id= tagId };


            var repoMock = new Mock<ITagRepository>();
            repoMock.Setup(r => r.GetByIdAsync(tagId)).ReturnsAsync(tag);
            repoMock.Setup(r => r.Delete(tagId));

            var mapper = MapperFactory.Create();

            var service = new TagService(repoMock.Object, mapper);


            // Act
            var result = await service.DeleteTag(tagId);

            //Assert
            Assert.True(result);
            repoMock.Verify(r => r.Delete(tagId), Times.Once);
        }

        [Fact]
        public async Task DeleteTag_ShouldThrow_WhenTagDoesnotExists()
        { 
            // Arrange
            var tagId = Guid.NewGuid();
            var repoMock = new Mock<ITagRepository>();
            repoMock.Setup(r => r.GetByIdAsync(tagId)).ReturnsAsync((Tag)null);

            var mapper = MapperFactory.Create();
            var service = new TagService(repoMock.Object, mapper);

            // Act
            var caughtException = await Assert.ThrowsAsync<EntityNotFoundException>(() => service.DeleteTag(tagId));

            // Assert
            Assert.Equal($"Impossible to find tag with id = '{tagId}'", caughtException.Message);
        }
    }
}
