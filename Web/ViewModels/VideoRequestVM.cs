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

        [StringLength(500, MinimumLength = 6)]
        public string? Description { get; set; } = string.Empty;

        public IFormFile? Img { get; set; }

        [Required]
        public string Thumbnail { get; set; } = string.Empty;

        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CategoryId == null && string.IsNullOrWhiteSpace(CategoryName))
            {
                yield return new ValidationResult(
                    "Either CategoryId or CategoryName must be provided.",
                    new[] { nameof(CategoryId), nameof(CategoryName) }
                );
            }
        }
    }
}
