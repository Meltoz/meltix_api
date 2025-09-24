using Application.Interfaces.Repository;
using Application.Services;
using Domain.Entities;
using Moq;

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
                

            var service = new TagService(repoMock.Object, null);


            // Act
            var result = await service.DeleteTag(tagId);

            //Assert
            Assert.True(result);
            repoMock.Verify(r => r.Delete(tagId), Times.Once);
        }
    }
}
