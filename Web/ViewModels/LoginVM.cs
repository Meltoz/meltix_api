using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class LoginVM
    {
        [Required]
        [StringLength(200, MinimumLength =3)]
        public string Pseudo { get; set; } = string.Empty;

        [Required]
        [StringLength(1000, MinimumLength = 3)]
        public string Password { get; set; } = string.Empty;
    }
}
