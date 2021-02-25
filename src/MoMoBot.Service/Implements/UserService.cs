using MoMoBot.Infrastructure.Database;
using MoMoBot.Service.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using MoMoBot.Service.Dtos;
using Microsoft.AspNetCore.Identity;
using MoMoBot.Infrastructure.Models;

namespace MoMoBot.Service.Implements
{
    public class UserService : IUserService
    {
        private readonly MoMoDbContext _context;
        private readonly UserManager<MoMoBotUser> _userManager;
        private readonly IAppSettings _appSettings;
        public UserService(MoMoDbContext context,
            UserManager<MoMoBotUser> userManager,
            IAppSettings appSettings)
        {
            _userManager = userManager;
            _context = context;
            _appSettings = appSettings;
        }

        public async Task<PagingQueryResult<UserItemDto>> GetAllUserPagingQueryAsync(int pageIndex = 1, int pageSize = 10)
        {
            var result = new PagingQueryResult<UserItemDto>();
            result.Total = await _context.Users.CountAsync();
            var skip = (pageIndex - 1) * pageSize;

            var details = from user in
                            (from user1 in _context.Users
                                 //orderby user1.UserName
                             select user1).Skip(skip).Take(pageSize)
                          join userrole in _context.UserRoles
                            on user.Id equals userrole.UserId into ur_join
                          from UR in ur_join.DefaultIfEmpty()
                          join role in _context.Roles
                            on UR.RoleId equals role.Id into r_join
                          from R in r_join.DefaultIfEmpty()
                          select new UserDetailDto
                          {
                              Id = user.Id,
                              RoleName = R.NormalizedName,
                              UserName = user.UserName,
                              Email = user.Email,
                              Nickname = user.Nickname
                          };

            var userRole = new Dictionary<string, List<string>>();
            if (details != null)
            {
                foreach (var item in details)
                {
                    if (userRole.ContainsKey(item.Id))
                    {
                        userRole[item.Id].Add(item.RoleName ?? "");
                    }
                    else
                    {
                        userRole.Add(item.Id, new List<string> { item.RoleName ?? "" });
                    }
                }
            }

            //var users = await details.Select(r => new UserItemDto
            //{
            //    Email = r.Email,
            //    Id = r.Id,
            //    UserName = r.UserName,
            //    LockoutEnabled = r.LockoutEnabled,
            //    Roles = userRole[r.Id],
            //    Nickname = r.Nickname
            //}).GroupBy(u => u.Id)
            //.Select(g => g.First())
            //.OrderBy(u => u.UserName)
            //.ToListAsync();

            var users2 = from r in
                             from u in
                                 from user in details
                                 select new UserItemDto
                                 {
                                     Email = user.Email,
                                     Id = user.Id,
                                     UserName = user.UserName,
                                     LockoutEnabled = user.LockoutEnabled,
                                     Roles = userRole[user.Id],
                                     Nickname = user.Nickname
                                 }
                             group u by u.Id into g
                             select g.First()
                         orderby r.UserName
                         select r;

            result.Data = users2;

            return result;
        }

        public async Task<IdentityResult> CreateUserAsync(UserDto dto, string password = null)
        {
            var emailExisted = await EmailExistedAsync(dto.Email);
            var nameExisted = await UsernameExistedAsync(dto.UserName);
            if (emailExisted)
            {
                return IdentityResult.Failed(new IdentityError { Code = "EmailExisted", Description = $"邮箱{dto.Email}已经存在！" });
            }
            if (nameExisted)
            {
                return IdentityResult.Failed(new IdentityError { Code = "UsernameExisted", Description = $"用户名{dto.UserName}已经存在！" });
            }

            var user = new MoMoBotUser
            {
                Avatar = await _appSettings.GetDefaultUserAvatarAsync(),
                Nickname = dto.Nickname,
                Email = dto.Email,
                UserName = dto.UserName
            };
            if (string.IsNullOrEmpty(password))
            {
                password = await _appSettings.GetDefaultUserPasswordAsync();
            }
            return await _userManager.CreateAsync(user, password);
        }

        private async Task<bool> EmailExistedAsync(string email)
        {
            var normalizeEmail = _userManager.NormalizeKey(email);
            return await _context.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizeEmail) != null;
        }
        private async Task<bool> UsernameExistedAsync(string username)
        {
            var normalizeName = _userManager.NormalizeKey(username);
            return await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizeName) != null;
        }

        public async Task<bool> ExistedAsync(string nameOrEmail)
        {
            var normalizeValue = _userManager.NormalizeKey(nameOrEmail);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizeValue || u.NormalizedEmail == normalizeValue);
            if (user == null)
            {
                return false;
            }
            return true;
        }
    }
}
