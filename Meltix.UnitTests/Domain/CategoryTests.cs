using Domain.Entities;

namespace Meltix.UnitTests.Domain
{
    public class CategoryTests
    {
        [Fact]
        public void Category_WithName_CreateComplete()
        {
            // Arrange
            var categoryName = "Test";

            // Act
            var category = new Category(categoryName); 

            // Assert
            Assert.NotNull(category);
            Assert.Equal(categoryName, category.Name);
        }

    }
}
