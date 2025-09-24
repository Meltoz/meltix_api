using Domain.Entities;
using Infrastructure.Data.Repositories;

namespace Meltix.IntegrationTests.Infrastructure
{
    public class VideoRepositoryTests
    {
        [Fact]
        public async Task GetBySlug_ReturnVideo()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var videoRepository = new VideoRepository(context);
            var video = new Video("test.mp4", "test.jpg", 10);
            var slug = "test";
            videoRepository.Insert(video);

            // Act
            var assertVideo = await videoRepository.GetBySlug(slug);

            // Assert
            Assert.NotNull(assertVideo);
            Assert.Equal("test", assertVideo.Slug);
        }

    }
}
