using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MoMoBot.Api.ViewModels;
using MoMoBot.Infrastructure;
using MoMoBot.Infrastructure.Extensions;
using MoMoBot.Infrastructure.Models;
using reCAPTCHA.AspNetCore;

namespace MoMoBot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IHostingEnvironment _env;
        private readonly UserManager<MoMoBotUser> _userManager;
        private readonly TokenValidationParameters _validationParameters;
        private readonly IRecaptchaService _recaptcha;
        public AccountController(UserManager<MoMoBotUser> userManager,
            TokenValidationParameters validationParameters,
            IHostingEnvironment env,
            IRecaptchaService recaptcha)
        {
            _userManager = userManager;
            _validationParameters = validationParameters;
            _recaptcha = recaptcha;
            _env = env;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]LoginViewModel vm)
        {
            if (!await GoogleValidation(vm.Code))
            {
                return BadRequest("人机验证失败，请稍后重试");
            }

            var user = await _userManager.FindByEmailAsync(vm.Email);

            if (user == null)
                return BadRequest("用户名或密码错误！");
            if (await _userManager.CheckPasswordAsync(user, vm.Password) == false)
            {
                return BadRequest("用户名或密码错误！");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Constants.Secret);
            var auth_time = DateTime.UtcNow;
            var expiresTime = vm.Remember ? TimeSpan.FromDays(3) : TimeSpan.FromMinutes(30);
            var expires_at = auth_time.Add(expiresTime);
            var claims = await GetUserClaimsAsync(user);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires_at,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var roles = await _userManager.GetRolesAsync(user);

            // return basic user info (without password) and token to store client side
            return Ok(new
            {
                userInfo = new
                {
                    user.Id,
                    user.Email,
                    username = user.UserName,
                    user.Nickname,
                    user.Avatar,
                    roles
                },
                token = tokenString,
                expires_at,
                auth_time
            });
        }

        [HttpPost("changepassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordViewModel vm)
        {
            var identity = HttpContext.IdentityUser<MoMoBotUser>(_validationParameters);
            if (identity != null)
            {
                var user = await _userManager.FindByIdAsync(identity.Id);
                if (user != null)
                {
                    var result = await _userManager.ChangePasswordAsync(user, vm.OldPassword, vm.NewPassword);
                    if (result.Succeeded)
                    {
                        return Ok();
                    }
                    return BadRequest(result.Errors);
                }
            }
            return Unauthorized();
        }

        private async Task<IList<Claim>> GetUserClaimsAsync(MoMoBotUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Audience,"momobot.api"),
                new Claim(JwtClaimTypes.Issuer,"yunstorm"),
                new Claim(JwtClaimTypes.Id, user.Id.ToString()),
                new Claim(JwtClaimTypes.Name, user.UserName),
                new Claim(JwtClaimTypes.Email, user.Email),
                new Claim(JwtClaimTypes.NickName, user.Nickname),
                new Claim("avatar", user.Avatar)
            };

            var roles = await _userManager.GetRolesAsync(user);
            if (roles != null)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, string.Join(',', roles)));
            }

            return claims;
        }

        /// <summary>
        /// google reCAPTCHA 验证
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private async Task<bool> GoogleValidation(string code)
        {
            //if(_env.IsDevelopment())
            //{
            //    return true;
            //}
            //var result = await _recaptcha.Validate(code);
            //if (result.success && result.score >= 0.7m)
            //{
            //    return true;
            //}
            //return false;

            return true;
        }
    }
}
