using Application.DTOs;
using Application.Interfaces.Repository;
using AutoMapper;
using System.Security.Claims;

namespace Application.Services
{
    public class TokenService(IMapper m, JwtService jwts, AesEncryptionService es, ITokenRepository tr)
    {
        private readonly IMapper _mapper = m;
        private readonly JwtService _jwtService = jwts;
        private readonly AesEncryptionService _encryptionService = es;
        private readonly ITokenRepository _tokenRepository = tr;

        private readonly static int daysRefreshExpire = 30;
        private readonly static int hourAccessExpire = 30;

        public async Task<TokenDTO> CreateTokenAsync(Guid userId, IEnumerable<Claim> claims)
        {
            var expires = DateTime.UtcNow.AddDays(daysRefreshExpire);
            var expiresAccess = DateTime.UtcNow.AddHours(hourAccessExpire);
            var accessToken = _jwtService.CreateAccessToken(claims, expiresAccess);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var token = await _tokenRepository.AddTokenAsync(accessToken, refreshToken, expires, userId);

            var tokenDto =  _mapper.Map<TokenDTO>(token);
            tokenDto.AccessExpiresAt = expiresAccess;

            return tokenDto;
        }

        public async Task RevokeAllToken(Guid userId)
        {
            await _tokenRepository.RevokeTokenFromUserIdAsync(userId);
        }

        public async Task RevokeTokenById(Guid tokenId)
        {
            await _tokenRepository.RevokeTokenAsync(tokenId);
        }

        public async Task<TokenDTO> GetTokenByRefreshToken(string refreshToken)
        {
            string encryptedRefreshToken = _encryptionService.Encrypt(refreshToken);
            var token = await _tokenRepository.GetByRefreshToken(encryptedRefreshToken);

            if (token is null)
                throw new EntryPointNotFoundException("Token not found");

            return _mapper.Map<TokenDTO>(token);
        }
    }
}
