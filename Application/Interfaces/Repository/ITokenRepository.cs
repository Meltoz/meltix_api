using Domain.Entities;

namespace Application.Interfaces.Repository
{
    public interface ITokenRepository : IRepository<TokenInfo>
    {
        public Task<TokenInfo?> AddTokenAsync(string accessToken, string refreshToken, DateTime expiresAt, Guid userId);

        public Task RevokeTokenAsync(Guid tokenId);

        public Task RevokeTokenFromUserAsync(User user);

        public Task RevokeTokenFromUserIdAsync(Guid userId);

        public Task<IEnumerable<TokenInfo>> GetActiveTokenFromUserIdAsync(Guid userId);

        public Task<IEnumerable<TokenInfo>> GetAllTokensByUserIdAsync(Guid userId);

        public Task<TokenInfo?> GetByRefreshToken(string refreshToken);
    }
}
