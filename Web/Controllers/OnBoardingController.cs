using Application.DTOs;
using Application.Services;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Enums.Sorting;
using Shared.Enums.Sorting.User;
using System.Security.Claims;
using Web.Constantes;
using Web.Extensions;
using Web.ViewModels;

namespace Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class OnBoardingController(UserService us, TokenService ts) : ControllerBase
    {
        private readonly UserService _userService = us;
        private readonly TokenService _tokenService = ts;

        [HttpGet]
        public async Task<IActionResult> CheckOnboarding()
        {
            var sortOption = new SortOption<SortUser>
            {
                Direction = SortDirection.Ascending,
                SortBy = SortUser.Role
            };

            var user = await _userService.PaginateAsync(0, 1, sortOption, true);

            return user.TotalCount == 0 ? Ok(false) : Ok(true);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFirstAdmin([FromBody] UserCreationVM userToCreate)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var sortOption = new SortOption<SortUser>
            {
                Direction = SortDirection.Ascending,
                SortBy = SortUser.Role
            };

            var user = await _userService.PaginateAsync(0, 1, sortOption, true);
            if(user.TotalCount > 0)
            {
                return BadRequest();
            }

            var admin =  await _userService.CreateUser(userToCreate.Pseudo, userToCreate.Password, RoleDTO.Admin);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Name, admin.Pseudo),
                new Claim(ClaimTypes.Role, admin.Role.ToString())
            };

            var tokens = await _tokenService.CreateTokenAsync(admin.Id, claims);

            Response.AppendCookie(ApiConstantes.AccessTokenCookieName, tokens.AccessToken, TimeSpan.FromHours(5), false);
            Response.AppendCookie(ApiConstantes.RefreshTokenCookieName, tokens.RefreshToken, TimeSpan.FromDays(30));

            return Ok();
        }
    }
}
