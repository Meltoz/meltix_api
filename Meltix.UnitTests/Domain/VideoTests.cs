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

        #region ChangePath Tests
        [Fact]
        public void ChangePath_WithValidPath_ShouldUpdatePath()
        {
            // Arrange
            var instance = new Video();
            string validPath = @"C:\Users\Documents";

            // Act
            instance.ChangePath(validPath);

            // Assert
            Assert.Equal(validPath, instance.Path);
        }

        [Fact]
        public void ChangePath_WithNullPath_ShouldThrowArgumentException()
        {
            // Arrange
            var instance = new Video();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => instance.ChangePath(null));
            Assert.Equal("path", exception.Message);
        }

        [Fact]
        public void ChangePath_WithEmptyPath_ShouldThrowArgumentException()
        {
            // Arrange
            var instance = new Video();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => instance.ChangePath(string.Empty));
            Assert.Equal("path", exception.Message);
        }

        [Fact]
        public void ChangePath_WithWhitespacePath_ShouldThrowArgumentException()
        {
            // Arrange
            var instance = new Video();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => instance.ChangePath("   "));
            Assert.Equal("path", exception.Message);
        }

        [Theory]
        [InlineData(@"C:\Program Files")]
        [InlineData(@"D:\Data\Files")]
        [InlineData("/home/user/documents")]
        [InlineData("relative/path")]
        public void ChangePath_WithVariousValidPaths_ShouldUpdatePath(string path)
        {
            // Arrange
            var instance = new Video();

            // Act
            instance.ChangePath(path);

            // Assert
            Assert.Equal(path, instance.Path);
        }

        [Fact]
        public void ChangePath_WithTabsOnly_ShouldThrowArgumentException()
        {
            // Arrange
            var instance = new Video();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => instance.ChangePath("\t\t"));
            Assert.Equal("path", exception.Message);
        }

        [Fact]
        public void ChangePath_WithNewLinesOnly_ShouldThrowArgumentException()
        {
            // Arrange
            var instance = new Video();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => instance.ChangePath("\n\r\n"));
            Assert.Equal("path", exception.Message);
        }

        [Fact]
        public void ChangePath_CalledMultipleTimes_ShouldUpdateToLatestPath()
        {
            // Arrange
            var instance = new Video();
            string firstPath = @"C:\First";
            string secondPath = @"C:\Second";

            // Act
            instance.ChangePath(firstPath);
            instance.ChangePath(secondPath);

            // Assert
            Assert.Equal(secondPath, instance.Path);
        }

        [Theory]
        [InlineData("a")] // Chemin minimal (1 caractère)
        [InlineData(" valid ")] // Espaces au début/fin (valide car contient du texte)
        public void ChangePath_WithEdgeCaseValidPaths_ShouldUpdatePath(string path)
        {
            // Arrange
            var instance = new Video();

            // Act
            instance.ChangePath(path);

            // Assert
            Assert.Equal(path, instance.Path);
        }

        [Fact]
        public void ChangePath_WithVeryLongPath_ShouldUpdatePath()
        {
            // Arrange
            var instance = new Video();
            string longPath = new string('a', 500); // Chemin très long

            // Act
            instance.ChangePath(longPath);

            // Assert
            Assert.Equal(longPath, instance.Path);
        }

        [Theory]
        [InlineData(@"C:\Path\With\Special\Chars\[brackets]")]
        [InlineData(@"C:\Path With Spaces\file.txt")]
        [InlineData(@"\\network\share\path")]
        [InlineData("path/with/special/chars/@#$%")]
        public void ChangePath_WithSpecialCharacters_ShouldUpdatePath(string path)
        {
            // Arrange
            var instance = new Video();

            // Act
            instance.ChangePath(path);

            // Assert
            Assert.Equal(path, instance.Path);
        }
        #endregion
    }
}
