using Application.Interfaces.Repository;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Shared.Exceptions;

namespace Infrastructure.Data.Repositories
{
    public class TokenRepository(MeltixContext c) : GenericRepository<TokenInfo>(c), ITokenRepository
    {
        public async Task<TokenInfo?> AddTokenAsync(string accessToken, string refreshToken, DateTime expiresAt, Guid userId)
        {
            var tokenAccess = new Token(accessToken);
            var tokenRefresh = new Token(refreshToken);

            var token = new TokenInfo(tokenAccess, tokenRefresh, userId, expiresAt);

            return await InsertAsync(token);
        }

        public async Task<IEnumerable<TokenInfo>> GetActiveTokenFromUserIdAsync(Guid userId)
        {
            var query = _dbSet.Where(t => t.UserId == userId && !t.IsRevoked);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TokenInfo>> GetAllTokensByUserIdAsync(Guid userId)
        {
            var query = _dbSet.Where(t => t.UserId == userId);

            return await query.ToListAsync();
        }

        public async Task<TokenInfo?> GetByRefreshToken(string encryptRefreshToken)
        {
            return await _dbSet.Where(t => t.RefreshToken == encryptRefreshToken).FirstOrDefaultAsync();
            
        }

        public async Task RevokeTokenAsync(Guid tokenId)
        {
            var token = await _dbSet.Where(t => t.Id == tokenId).SingleOrDefaultAsync();

            if (token == null)
                throw new EntityNotFoundException($"Token with id = '{tokenId}' not found");

            token.RevokeToken();
            await UpdateAsync(token);
        }

        public async Task RevokeTokenFromUserAsync(User user)
        {
            await RevokeTokenFromUserIdAsync(user.Id);
        }

        public async Task RevokeTokenFromUserIdAsync(Guid userId)
        {
            var tokens = await _dbSet.Where(t => t.UserId == userId).ToListAsync();

            foreach (var token in tokens)
            {
                token.RevokeToken();
                await UpdateWithOutSaveAsync(token);
            }
            await SaveAsync();
        }
    }
}
