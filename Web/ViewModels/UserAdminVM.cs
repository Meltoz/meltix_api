namespace Web.ViewModels
{
    public class UserAdminVM
    {
        public Guid Id { get; set; }

        public string Pseudo { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public DateTime? LastLoggin { get; set; }

        public DateTime? LastChangePassword { get; set; }
    }
}
