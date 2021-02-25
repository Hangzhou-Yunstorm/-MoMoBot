using Microsoft.EntityFrameworkCore.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoMoBot.Infrastructure.Database;
using MoMoBot.Service.Abstractions;
using MoMoBot.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MoMoBot.Service.Implements
{
    public class SystemLogReader : ISystemLogReader
    {
        private readonly LogDbContext _dbContext;
        public SystemLogReader(LogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagingQueryResult<Log>> GetLogsAsync(List<int> levels, DateTime? start, DateTime? end, int offset = 0, int limit = 20)
        {
            var result = new PagingQueryResult<Log>();
            var where = new StringBuilder();
            var hasWhere = false;

            if (levels?.Count > 0)
            {
                var level = string.Join(",", levels);
                where.Append($"WHERE \"level\" IN({level}) ");
                hasWhere = true;
            }

            if (start.HasValue && end.HasValue)
            {
                if (hasWhere)
                {
                    where.Append($"AND to_char(\"timestamp\",'YYYY-MM-DD') BETWEEN {start.Value.ToString("yyyy-MM-dd")} AND {end.Value.ToString("yyyy-MM-dd")}");
                }
                else
                {
                    where.Append($"WHERE to_char(\"timestamp\",'YYYY-MM-DD') BETWEEN {start.Value.ToString("yyyy-MM-dd")} AND {end.Value.ToString("yyyy-MM-dd")}");
                }
            }


            var query = $"SELECT message, \"level\", \"timestamp\", \"exception\", log_event FROM system_logs {where} ORDER BY \"timestamp\" DESC OFFSET @offset LIMIT @limit";
            var count = $"SELECT count(*) from system_logs {where}";
            result.Data = await _dbContext.SqlQueryAsync<Log>(query, new { levels, offset, limit });
            result.Total = await _dbContext.SqlQueryFirstOrDefaultAsync<int>(count, new { levels });
            return result;
        }

        public async Task<int> GetErrorCountAsync()
        {
            var sql = $"SELECT count(*) from system_logs WHERE \"level\" IN (4,5)";
            return await _dbContext.SqlQueryFirstOrDefaultAsync<int>(sql);
        }
    }
}
