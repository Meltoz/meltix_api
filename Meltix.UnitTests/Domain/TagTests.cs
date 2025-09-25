using Domain.Entities;

namespace Meltix.UnitTests.Domain
{
    public class TagTests
    {
        [Fact]
        public void Tag_ShouldCreate_WhenName()
        {
            // Arrange
            var tagName = "test";

            // Act
            var tag = new Tag(tagName);

            // Assert
            Assert.NotNull(tag);
            Assert.Equal(tagName, tag.Value);
        }

        [Fact]
        public void Tag_ShouldThrow_WhenTagNameWhiteSpace()
        {
            // Arrange
            var tagName = " ";

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => new Tag(tagName));

            // Assert
            Assert.IsType<ArgumentException>(caughtException);
        }

        [Fact]
        public void Tag_ShouldThrow_WhenTagNameEmpty()
        {
            // Arrange
            var tagName = "";

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => new Tag(tagName));

            // Assert
            Assert.IsType<ArgumentException>(caughtException);
        }

        [Fact]
        public void Tag_ShouldThrow_WhenTagNameNull()
        {
            // Arrange
            string tagName = null;

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => new Tag(tagName));

            // Assert
            Assert.IsType<ArgumentException>(caughtException);
        }

        [Fact]
        public void ChangeValue_ShouldChange_WhenTagName()
        {
            // Arrange
            var tagName = "test";
            var newtagName = "test2";
            var tag = new Tag(tagName);

            // Act
            tag.ChangeValue(newtagName);

            // Assert
            Assert.NotNull(tag.Value);
            Assert.Equal(newtagName, tag.Value);
        }

        [Fact]
        public void ChangeValue_ShouldThrow_WhenWhiteSpace()
        {
            // Arrange
            var tagName = "test";
            var newtagName = "  ";
            var tag = new Tag(tagName);

            // Act
            var caughtException =  Assert.ThrowsAny<Exception>(() => tag.ChangeValue(newtagName));

            // Assert
            Assert.IsType<ArgumentException>(caughtException);
        }

        [Fact]
        public void ChangeValue_ShouldThrow_WhenEmpty()
        {
            // Arrange
            var tagName = "test";
            var newtagName = string.Empty;
            var tag = new Tag(tagName);

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => tag.ChangeValue(newtagName));

            // Assert
            Assert.IsType<ArgumentException>(caughtException);
        }

        [Fact]
        public void ChangeValue_ShouldThrow_WhenNull()
        {
            // Arrange
            var tagName = "test";
            string newtagName = null;
            var tag = new Tag(tagName);

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => tag.ChangeValue(newtagName));

            // Assert
            Assert.IsType<ArgumentException>(caughtException);
        }

    }
}
