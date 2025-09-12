using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class VideoRequestVM :IValidatableObject
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, MinimumLength = 0)]
        public string? Description { get; set; } = string.Empty;

        public IFormFile? Img { get; set; }

        public Guid? CategoryId { get; set; }
        public string? Category { get; set; }

        public string? Tags { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CategoryId == null && string.IsNullOrWhiteSpace(Category))
            {
                yield return new ValidationResult(
                    "Either CategoryId or CategoryName must be provided.",
                    new[] { nameof(CategoryId), nameof(Category) }
                );
            }
        }
    }
}
