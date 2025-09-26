namespace Application.DTOs
{
    public class UserDTO
    {
        public string Pseudo { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public RoleDTO Role { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime? LastChangePassword { get; set; } 

        public DateTime? Updated { get; set; }
    }
}
