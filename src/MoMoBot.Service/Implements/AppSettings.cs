using EasyCaching.Core;
using Microsoft.EntityFrameworkCore;
using MoMoBot.Core;
using MoMoBot.Infrastructure;
using MoMoBot.Infrastructure.Database;
using MoMoBot.Service.Abstractions;
using MoMoBot.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoMoBot.Service.Implements
{
    public class AppSettings : IAppSettings
    {
        private readonly MoMoDbContext _dbContext;
        private readonly IEasyCachingProvider _cache;
        public AppSettings(MoMoDbContext dbContext,
            IEasyCachingProvider cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }


        public async Task<string> GetAsync(string key)
        {
            MoMoBotAssert.KeyNotNullOrEmpty(key);
            var settings = (await _cache.GetAsync<List<AppSettingsDto>>(Constants.RedisKey.Settings))?.Value;
            if (settings == null)
            {
                settings = await GetSettingsFromDBAsync();
            }
            return settings.FirstOrDefault(s => s.Key == key)?.Value;
        }

        public async Task SetAsync(string key, string value)
        {
            MoMoBotAssert.KeyNotNullOrEmpty(key);
            MoMoBotAssert.ValueNullOrWhiteSpace(value);

            var setting = await _dbContext.Settings.FirstOrDefaultAsync(s => s.Key == key);
            if (setting != null)
            {
                if (setting.ReadOnly == false)
                {
                    setting.Value = value;
                    _dbContext.Settings.Update(setting);
                    await _dbContext.SaveChangesAsync();
                    await GetSettingsFromDBAsync();
                }
            }
        }


        public async Task<string> GetDefaultUserPasswordAsync() => await GetAsync(SettingKeys.DefaultUserPassword);

        public async Task<string> GetDefaultUserAvatarAsync() => await GetAsync(SettingKeys.DefaultUserAvatar);

        public async Task<string> GetDTalkAppNameAsync() => await GetAsync(SettingKeys.DTalkAppName);

        /// <summary>
        /// 获取设置并且同步缓存中的设置
        /// </summary>
        /// <returns></returns>
        private async Task<List<AppSettingsDto>> GetSettingsFromDBAsync()
        {
            var settings = await _dbContext.Settings.Select(s => new AppSettingsDto
            {
                Key = s.Key,
                Value = s.Value
            }).ToListAsync();
            await _cache.SetAsync(Constants.RedisKey.Settings, settings, TimeSpan.FromHours(1));
            return settings;
        }

        public async Task ClearCacheAsync() => await _cache.RemoveAsync(Constants.RedisKey.Settings);

        public async Task<List<AppSettingsDto>> GetAllAsync()
        {
            var settings = (await _cache.GetAsync<List<AppSettingsDto>>(Constants.RedisKey.Settings))?.Value;
            if (settings == null)
            {
                settings = await GetSettingsFromDBAsync();
            }
            return settings;
        }

        public async Task<double> GetIntentMinimumMatchingDegreeAsync(double defValue)
        {
            var degree = (await _cache.GetAsync<double>(Constants.RedisKey.IntentMinimumMatchingDegree))?.Value;
            if (degree.HasValue == false || degree <= 0)
            {
                var str = await GetAsync(SettingKeys.IntentMinimumMatchingDegree);
                if (!double.TryParse(str, out var result))
                {
                    result = defValue;
                }
                degree = result;
                await _cache.SetAsync(Constants.RedisKey.IntentMinimumMatchingDegree, degree, TimeSpan.FromHours(1));
            }

            return degree.Value;
        }
    }

    public static class SettingKeys
    {
        public const string BusinessInquiryUrl = "business_inquiry_url";
        public const string DefaultUserPassword = "default_user_password";
        public const string DefaultUserAvatar = "default_user_avatar";
        public const string IntentMinimumMatchingDegree = "intent_minimum_matching_degree";
        public const string DTalkAppName = "dtalk_app_name";
    }

}
