using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MoMoBot.Infrastructure;
using MoMoBot.Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Extensions;
using Microsoft.Extensions.Logging;

namespace MoMoBot.Api.BackgroundTasks
{
    /// <summary>
    /// 热门意图统计
    /// </summary>
    public class StatisticsMatchIntentCountTask
    {
        private readonly IRedisCachingProvider _redis;
        private readonly MoMoDbContext _dbContext;
        private readonly ILogger<StatisticsMatchIntentCountTask> _logger;
        public StatisticsMatchIntentCountTask(IRedisCachingProvider redis,
            IConfiguration configuration,
            ILogger<StatisticsMatchIntentCountTask> logger)
        {
            _redis = redis;
            _logger = logger;
            var connection = configuration.GetConnectionString("DefaultConnection");
            var options = new DbContextOptionsBuilder<MoMoDbContext>()
                .UseNpgsql(connection)
                .Options;
            _dbContext = new MoMoDbContext(options);
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("开始执行【StatisticsMatchIntentCountTask】任务...");

            var length = await _redis.LLenAsync(Constants.RedisKey.MatchIntents);
            if (length > 0)
            {
                length = length > 20 ? 20 : length;
                var sql = new StringBuilder();
                var now = DateTime.Now;
                var existedIntents = new List<string>();
                for (var i = 0; i < length; i++)
                {
                    var intent = await _redis.LPopAsync<string>(Constants.RedisKey.MatchIntents);
                    if (string.IsNullOrEmpty(intent))
                    {
                        break;
                    }

                    if (existedIntents.Any(e => e == intent) ||
                        _dbContext.HotIntents.Any(h => h.Intent == intent))
                    {
                        sql.Append($"UPDATE \"HotIntents\" SET \"Count\" = \"Count\"+1, \"UpdateTime\" ='{now}' WHERE \"Intent\"='{intent.Replace('\'', ' ')}';");
                    }
                    else
                    {
                        sql.Append($"INSERT INTO \"HotIntents\" (\"Intent\",\"Count\",\"UpdateTime\") VALUES ('{intent.Replace('\'', ' ')}',1,'{now}');");
                    }
                    existedIntents.Add(intent);
                }
                if (sql.Length > 0)
                {
                    await _dbContext.ExecuteSqlAsync(sql.ToString());
                }
            }
            _logger.LogInformation($"记录{length}条");
        }
    }
}
