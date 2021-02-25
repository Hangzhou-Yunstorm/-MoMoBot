using MoMoBot.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MoMoBot.Service.Abstractions
{
    public interface IAppSettings
    {
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<string> GetAsync(string key);
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task SetAsync(string key, string value);
        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <returns></returns>
        Task ClearCacheAsync();
        /// <summary>
        /// 获取用户默认密码
        /// </summary>
        /// <returns></returns>
        Task<string> GetDefaultUserPasswordAsync();
        /// <summary>
        /// 获取用户默认头像
        /// </summary>
        /// <returns></returns>
        Task<string> GetDefaultUserAvatarAsync();
        /// <summary>
        /// 获取钉钉应用名称
        /// </summary>
        /// <returns></returns>
        Task<string> GetDTalkAppNameAsync();
        /// <summary>
        /// 获取所有设置
        /// </summary>
        /// <returns></returns>
        Task<List<AppSettingsDto>> GetAllAsync();

        /// <summary>
        /// 获取意图最小匹配度
        /// </summary>
        /// <param name="defValue">默认值</param>
        /// <returns></returns>
        Task<double> GetIntentMinimumMatchingDegreeAsync(double defValue);
    }
}
