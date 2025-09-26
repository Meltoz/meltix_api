using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Enums.Sorting.User;
using System.Text.RegularExpressions;
using Web.Constantes;
using Web.ViewModels;

namespace Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IMapper _mapper;

        public UserController(UserService us, IMapper m)
        {
            _userService = us;
            _mapper = m;
        }

        [HttpGet]
        public async Task<IActionResult> Search(int pageIndex, int pageSize, string sort, bool onlyAdmin, string? pseudo)
        {
            if (pageIndex < 0 || pageSize < 1)
                return BadRequest();

            if (!GetSorting(sort, out var sortOption))
                return BadRequest();

            var pagedResult = await _userService.PaginateAsync(pageIndex, pageSize, sortOption, onlyAdmin, pseudo);

            var users = _mapper.Map<IEnumerable<UserAdminVM>>(pagedResult.Data);
            
            Response.Headers.Append(ApiConstantes.HeaderTotalCount, pagedResult.TotalCount.ToString());
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]LoginRequestVM user)
        {
            if(!ModelState.IsValid)
                return BadRequest();

            var userCreated = await _userService.CreateUser(user.Pseudo, user.Password);

            return Ok(_mapper.Map<UserAdminVM>(userCreated));
        }

        [HttpPatch]
        public async Task<IActionResult> Update()
        {
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _userService.DeleteUserAsync(id);

            return Ok();
        }

        private bool GetSorting(string sort, out SortOption<SortUser> sortOption)
        {
            var patternSort = @"^(create|role|pseudo|lastlogin)_(ascending|descending)$";
            var regex = new Regex(patternSort);
            var match = regex.Match(sort);
            if (!match.Success)
            {
                sortOption = null;
                return false;
            }

            var field = match.Groups[1].Value;
            var direction = match.Groups[2].Value;
            sortOption = SortOptionFactory.Create<SortUser>(field, direction);
            return true;
        }
    }
}
