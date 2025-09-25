using Domain.Entities;

namespace Meltix.UnitTests.Domain
{
    public class CategoryTests
    {
        [Fact]
        public void Category_ShouldCreate_WhenName()
        {
            // Arrange
            var categoryName = "test";

            // Act
            var category = new Category(categoryName); 

            // Assert
            Assert.NotNull(category);
            Assert.Equal(categoryName, category.Name);
        }

        [Fact]
        public void Category_ShouldCreateWithLower_WhenNameInUpper()
        {
            // Arrange
            var categoryName = "test".ToUpper();

            // Act
            var category = new Category(categoryName);

            // Assert
            Assert.NotNull(category);
            Assert.Equal(categoryName.ToLower(), category.Name);
        }

        [Fact]
        public void ChangeName_ShouldUpdateName_WhenName()
        {
            // Arrange
            var categoryName = "test";
            var newCategoryName = "test2";
            var category = new Category(categoryName);

            // Act
            category.ChangeName(newCategoryName);

            // Assert
            Assert.NotNull(category);
            Assert.Equal(newCategoryName, category.Name);
        }

        [Fact]
        public void Category_ShouldThrow_WhenNoName()
        {
            // Arrange
            string categoryName = null;
            
            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => new Category(categoryName));

            // Assert
            Assert.IsType<ArgumentException>(caughtException);
        }

        [Fact]
        public void Category_ShouldThrow_WhenWhiteSpace()
        {
            // Arrange
            var categoryName = " ";

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => new Category(categoryName));

            // Assert
            Assert.IsType<ArgumentException>(caughtException);
        }

        [Fact]
        public void ChangeName_ShouldThrow_WhenSameName()
        {
            // Arrange
            var categoryName = "Test";
            var category = new Category(categoryName);

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => category.ChangeName(categoryName));

            // Assert
            Assert.IsType<ArgumentException>(caughtException);
        }

        [Fact]
        public void ChangeName_ShouldThrow_WhenNameInUppercase()
        {
            // Arrange
            var categoryNameLower = "test";
            var categoryNameUpper = categoryNameLower.ToUpper();
            var category = new Category(categoryNameLower);

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => category.ChangeName(categoryNameUpper));

            // Assert
            Assert.IsType<ArgumentException>(caughtException);
        }
    }
}
