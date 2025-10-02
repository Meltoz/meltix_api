using Application.DTOs;
using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Exceptions;
using System.Security.Claims;
using Web.Constantes;
using Web.ViewModels;

namespace Web.Controllers
{

    [ApiController]
    [AllowAnonymous]
    [Route("api")]
    public class AuthController(UserService us, JwtService jwts, IMapper m) : ControllerBase
    {
        private readonly UserService _userService = us;
        private readonly JwtService _jwtService = jwts;
        private readonly IMapper _mapper = m;


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

            var accessToken = _jwtService.CreateAccessToken(claims);
            Response.Cookies.Append(ApiConstantes.AccessTokenCookieName, accessToken, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(1),
                HttpOnly = false,
                Path = "/",
                Secure = true,
                SameSite = SameSiteMode.None,
                Domain = "localhost"
            });

            return Ok();
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh() 
        {
            return Ok();
        }

        [HttpDelete]
        [Route("logout")]
        public async Task<IActionResult> Logout() 
        {
            Response.Cookies.Append(ApiConstantes.AccessTokenCookieName, "", new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(-1),
                HttpOnly = false,
                Path = "/",
                Secure = true,
                Domain = "localhost"
            });

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
    }

   
}


