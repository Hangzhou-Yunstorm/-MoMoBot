using Microsoft.AspNetCore.Identity;
using MoMoBot.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MoMoBot.Service.Abstractions
{
    public interface IUserService
    {
        /// <summary>
        /// 获取用户列表分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PagingQueryResult<UserItemDto>> GetAllUserPagingQueryAsync(int pageIndex = 1, int pageSize = 10);
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<IdentityResult> CreateUserAsync(UserDto dto, string password = null);
        /// <summary>
        /// 根据用户名或者邮箱判断是否已存在
        /// </summary>
        /// <param name="nameOrEmail"></param>
        /// <returns></returns>
        Task<bool> ExistedAsync(string nameOrEmail);
    }
}
