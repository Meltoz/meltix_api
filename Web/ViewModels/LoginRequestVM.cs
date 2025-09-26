using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class LoginRequestVM
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Pseudo { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]
        public string Password { get; set; } = string.Empty;
    }
}
