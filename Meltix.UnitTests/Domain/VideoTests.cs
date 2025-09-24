using Domain.Entities;
using Shared;
using Shared.Exceptions;

namespace Meltix.UnitTests.Domain
{
    public class VideoTests
    {
        [Fact]
        public void Video_WithPath_CreateComplete()
        {
            // Arrange
            var path = "Avenger_end_game.mp4";
            var slug = SlugGenerator.Generate(path);

            // Act
            var video = new Video(path);

            // Assert
            Assert.NotNull(video);
            Assert.Equal(path, video.Path);
            Assert.Equal(slug, video.Slug);
        }

        [Fact]
        public void Video_WithNoPath_CreateFailed()
        {
            // Arrange
            var path = "";

            // Act
            var caughtException  = Assert.Throws<ArgumentException>(() => new Video(path));

            // Assert
            Assert.Equal("Path must be defined", caughtException.Message);
        }

        [Fact]
        public void Video_WithPathAndThumbnail_CreateComplete()
        {
            // Arrange
            var path = "Avenger_end_game.mp4";
            var slug = SlugGenerator.Generate(path);
            var thumbnail = "avenger.jpg";

            // Act
            var video = new Video(path, thumbnail);

            // Assert
            Assert.NotNull(video);
            Assert.Equal(path, video.Path);
            Assert.Equal(slug, video.Slug);
            Assert.Equal(thumbnail, video.Thumbnail);
        }

        public void Video_WithPathAndThumbnailAndDuration_CreateComplete()
        {
            // Arrange
            var path = "Avenger_end_game.mp4";
            var slug = SlugGenerator.Generate(path);
            var thumbnail = "avenger.jpg";
            var duration = 10;

            // Act
            var video = new Video(path, thumbnail, duration);

            // Assert
            Assert.NotNull(video);
            Assert.Equal(path, video.Path);
            Assert.Equal(slug, video.Slug);
            Assert.Equal(thumbnail, video.Thumbnail);
            Assert.Equal(duration, video.Duration);
        }

        [Fact]
        public void Video_WithPathAndThumbnailAndDurationNegative_CreateFail()
        {
            // Arrange
            var path = "Avenger_end_game.mp4";
            var slug = SlugGenerator.Generate(path);
            var thumbnail = "avenger.jpg";
            var duration = -10;

            // Act
            var caughtException = Assert.Throws<ArgumentException>(() => new Video(path, thumbnail, duration));

            // Assert
            Assert.Equal("Duration must be postive", caughtException.Message);
        }

        [Fact]
        public void AddTag_WithTag_CreateComplete()
        {
            // Arrange
            var video = new Video();
            var tag = new Tag();

            // Act
            video.AddTags(tag);

            // Assert
            Assert.NotNull(video.Tags);
            Assert.Single(video.Tags);
        }

        [Fact]
        public void AddTag_DoubleTag_ThrowException()
        {
            // Arrange
            var video = new Video();
            var tag = new Tag();

            // Act
            video.AddTags(tag);
            var caughtException = Assert.Throws<TagAlreadyExistException>(() => video.AddTags(tag));

            // Assert
            Assert.IsType<TagAlreadyExistException>(caughtException);
            Assert.Single(video.Tags);
        }

        [Fact]
        public void RemoveTag_WithTag_RemoveTag()
        {
            // Arrange
            var video = new Video();
            var tag = new Tag();
            video.AddTags(tag);

            // Act
            video.RemoveTags(tag);

            // Assert
            Assert.Empty(video.Tags);
        }

        [Fact]
        public void RemoveTag_WithNoTag_ThrowException()
        {
            // Arrange
            var video = new Video();
            var tag = new Tag();

            // Act
            var caughtException = Assert.Throws<EntityNotFoundException>(() => video.RemoveTags(tag));

            // Assert
            Assert.IsType<EntityNotFoundException>(caughtException);
        }

        [Fact]
        public void RemoveTag_RemoveDoubleTag_RemoveTag()
        {
            // Arrange
            var video = new Video();
            var tag = new Tag();
            video.AddTags(tag);

            // Act
            video.RemoveTags(tag);
            var caughtException = Assert.ThrowsAny<Exception>(() => video.RemoveTags(tag));

            // Assert
            Assert.IsType<EntityNotFoundException>(caughtException);
        }

        [Fact]
        public void ChangeTitle_WithTitle_TitleChanged()
        {
            // Arrange
            var video = new Video("title");
            var newTitle = "testing new Title";
            var newSlug = SlugGenerator.Generate(newTitle);

            // Act
            video.ChangeTitle(newTitle);

            // Assert
            Assert.Equal(newTitle, video.Title);
            Assert.Equal(newSlug, video.Slug);
        }

        [Fact]
        public void ChangeTitle_WithNoTitle_ThrowException()
        {
            // Arrange
            var video = new Video("title");
            var newTitle = "";

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => video.ChangeTitle(newTitle));

            // Assert
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("Title must be defined", caughtException.Message);
        }

        [Fact]
        public void ChangeTitle_WithWhiteSpace_ThrowException()
        {
            // Arrange
            var video = new Video("title");
            var newTitle = "  ";

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => video.ChangeTitle(newTitle));

            // Assert
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("Title must be defined", caughtException.Message);
        }
    }
}
