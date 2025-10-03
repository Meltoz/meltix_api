using Application.DTOs;
using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Configuration;
using Shared.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Web.Constantes;
using Web.Extensions;
using Web.ViewModels;

namespace Web.Controllers
{

    [ApiController]
    [AllowAnonymous]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly TokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly string _key;

        public AuthController(UserService us, TokenService ts, IMapper m, IOptions<AuthConfiguration> config)
        {
            _userService = us;
            _tokenService = ts;
            _mapper = m;
            _key = config.Value.Key;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginVM login)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            UserDTO user;
            try
            {
                user = await _userService.ValidateUser(login.Pseudo, login.Password);
            }
            catch (EntityNotFoundException)
            {
                return Unauthorized();
            }
            catch (Exception)
            {
                throw;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Pseudo),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            await _tokenService.RevokeAllToken(user.Id);
            var token = await _tokenService.CreateTokenAsync(user.Id, claims);

            Response.AppendCookie(ApiConstantes.AccessTokenCookieName, token.AccessToken, TimeSpan.FromHours(5), false, domain: "localhost");
            Response.AppendCookie(ApiConstantes.RefreshTokenCookieName, token.RefreshToken, TimeSpan.FromDays(30), domain: "localhost");

            return Ok();
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh() 
        {
            var oldRefreshToken = Request.Cookies[ApiConstantes.RefreshTokenCookieName];
            if (string.IsNullOrWhiteSpace(oldRefreshToken))
                return BadRequest();

            var storedToken = await _tokenService.GetTokenByRefreshToken(oldRefreshToken);

            var principal = GetPrincipalFromExpiredToken(storedToken.AccessToken);

            var userId = Guid.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (storedToken == null ||
                storedToken.IsRevoked ||
                storedToken.RefreshExpiresAt < DateTime.UtcNow)
            {
                await _tokenService.RevokeAllToken(userId);
                Response.DeleteCookie(ApiConstantes.AccessTokenCookieName, domain: "localhost");
                Response.DeleteCookie(ApiConstantes.RefreshTokenCookieName, domain: "localhost");
                return Unauthorized();
            }

            var token = await _tokenService.CreateTokenAsync(userId, principal.Claims);

            Response.AppendCookie(ApiConstantes.AccessTokenCookieName, token.AccessToken, expiryDuration: TimeSpan.FromHours(5), httpOnly: false, domain: "localhost");
            Response.AppendCookie(ApiConstantes.RefreshTokenCookieName, token.RefreshToken, expiryDuration: TimeSpan.FromDays(30), httpOnly: true, domain: "localhost");

            return Ok();
        }

        [HttpDelete]
        [Route("logout")]
        public async Task<IActionResult> Logout() 
        {
            Response.DeleteCookie(ApiConstantes.AccessTokenCookieName, httpOnly: false, domain: "localhost");
            Response.DeleteCookie(ApiConstantes.RefreshTokenCookieName, httpOnly: true,  domain: "localhost");

            return Ok();
        }

        [HttpGet]
        [Route("me")]
        public async Task<IActionResult> Me()
        {
            var userId = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;

            if (userId == Guid.Empty)
                return Unauthorized();

            var user = await _userService.GetByIdAsync(userId);

            return Ok(_mapper.Map<UserAdminVM>(user));
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidation = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidation, out var securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurity || !jwtSecurity.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }

   
}


