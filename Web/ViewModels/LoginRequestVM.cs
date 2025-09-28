using Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class LoginRequestVM
    {
        public Guid? Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Pseudo { get; set; } = string.Empty;

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]
        public string? Password { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(RoleDTO), ErrorMessage ="Role must be 'Admin' or 'User'")]
        public RoleDTO Role { get; set; } 
    }
}
