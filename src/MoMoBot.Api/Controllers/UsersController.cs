using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoMoBot.Api.ViewModels;
using MoMoBot.Service.Abstractions;
using MoMoBot.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUserPagingQueryAsync();
            return Ok(users);
        }

        [HttpPost("existed")]
        public async Task<IActionResult> Existed(string nameOrEmail)
        {
            var existed = await _userService.ExistedAsync(nameOrEmail);
            return Ok(existed);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody]CreateUserViewModel vm)
        {
            var user = new UserDto
            {
                Email = vm.Email,
                Nickname = vm.Nickname,
                UserName = vm.Username
            };
            var result = await _userService.CreateUserAsync(user, vm.Password);
            if (result.Succeeded == false)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
    }
}
