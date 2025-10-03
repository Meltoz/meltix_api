using Domain.ValueObjects;

namespace Domain.Entities
{
    public class TokenInfo : BaseEntity
    {
        public Token AccessToken { get; private set; }

        public Token RefreshToken { get; private set; }

        public bool IsRevoked { get; private set; }

        public Guid UserId { get; private set; }

        public User User { get; private set; }

        public DateTime ExpiresAt { get; private set; }


        public TokenInfo() { }

        public TokenInfo(Token accessToken, Token refreshToken, Guid userId, DateTime refreshExpires)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            IsRevoked = false;
            UserId = userId;
            ExpiresAt = refreshExpires;
        }

        public void RevokeToken()
        {
            IsRevoked = true;
        }
    }
}
