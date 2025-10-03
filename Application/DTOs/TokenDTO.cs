namespace Application.DTOs
{
    public class TokenDTO
    {
        public Guid Id { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshExpiresAt { get; set; }

        public DateTime AccessExpiresAt { get; set; }

        public bool IsRevoked { get; set; }

        public UserDTO User { get; set; }
    }
}
