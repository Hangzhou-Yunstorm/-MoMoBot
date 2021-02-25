using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions;
using MoMoBot.Infrastructure.Database;
using MoMoBot.Infrastructure.Extensions;
using MoMoBot.Infrastructure.Models;
using MoMoBot.Service.Abstractions;
using MoMoBot.Service.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoMoBot.Service.Implements
{
    public class ServiceRecordService
        : IServiceRecordService
    {
        private MoMoDbContext _context { get; }
        public ServiceRecordService(MoMoDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceRecord> AddAsync(ServiceRecord serviceRecord)
        {
            var entry = await _context.ServiceRecords.AddAsync(serviceRecord);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<ServiceRecord> FindTask(string userId, long id)
        {
            return await Task.FromResult(_context.ServiceRecords.FirstOrDefault(r => r.Id == id && r.UserId == userId));
        }

        public async Task<PaginationViewModel> GetPaginationAsync(string search, bool? lowScore, string userId, int offset, int limit, bool? done)
        {
            var result = new PaginationViewModel();
            var sqlWhere = new StringBuilder();
            sqlWhere.Append($" WHERE (\"UserId\"=@userId ");
            if (!string.IsNullOrEmpty(search))//是否含有关键字搜索
            {
                sqlWhere.Append($"AND (\"Title\" LIKE '%{search.ReplaceSqlChars()}%' OR \"Remarks\" LIKE '%{search.ReplaceSqlChars()}%') ");
            }
            if (done.HasValue && done.Equals(true)) //是否归档
            {
                sqlWhere.Append($"AND \"IsDone\" = {done} ");
            }
            if (lowScore.HasValue && lowScore.Equals(true))
            {
                sqlWhere.Append($"AND \"Score\" < 3 ");
            }
            sqlWhere.Append(")");
            //查询搜索总数
            string selectCount = "SELECT COUNT(*) FROM \"public\".\"ServiceRecords\"" + sqlWhere;
            int dataCount = await _context.SqlQueryFirstAsync<int>(selectCount, new { userId });

            //查询数据 
            var selectData = "SELECT * FROM \"public\".\"ServiceRecords\" " + sqlWhere + $" order by \"IsDone\", \"EndOfServiceTime\" LIMIT {limit} OFFSET {offset}";
            var data = await _context.SqlQueryAsync<ServiceRecord>(selectData, new { userId });
            result.Total = dataCount;
            result.Rows = data;
            return result;
        }

        public async Task ScoreAsync(long id, int score)
        {
            var record = _context.ServiceRecords.FirstOrDefault(r => r.Id == id && r.IsDone == false);
            if (record != null)
            {
                record.Score = score;
                _context.Entry(record).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task TaskCompletion(ServiceRecord record)
        {
            _context.Entry(record).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<List<ServiceRecord>> GetIsDone(string userId)
        {
            return await _context.ServiceRecords.Where(r => r.UserId == userId && r.IsDone == true && r.Score <= 3).ToListAsync();
        }

        public async Task<ServiceRecord> GenerateServiceRecordAsync(string serverId, long chatId)
        {
            if (await _context.ChatRecords.CountAsync(c => c.Chat.Id == chatId && c.Who != 0) > 0)
            {
                var record = new ServiceRecord
                {
                    EndOfServiceTime = DateTime.Now,
                    IsDone = false,
                    Score = 5,
                    UserId = serverId
                };
                await _context.AddAsync(record);
                //await _context.SaveChangesAsync();
                return record;
            }
            return null;
        }

        public async Task<IEnumerable<ServiceRecordsDto>> GetServiceRecordsAsync(string owner, string identityId, int limit = 10)
        {
            var sql = "SELECT a.\"ServiceRecordId\" AS \"Id\",a.\"Id\" AS \"ChatId\", a.\"Title\" AS \"Name\", b.\"Title\", b.\"IsDone\", b.\"EndOfServiceTime\", b.\"RevordCompletionTime\" AS \"RecordCompletionTime\", b.\"Score\", b.\"Remarks\" FROM \"Chats\" AS a LEFT JOIN \"ServiceRecords\" AS b ON a.\"ServiceRecordId\" = b.\"Id\" WHERE b.\"Deleted\"=false AND \"Owner\" = @owner AND \"Other\" = @identityId AND \"ServiceRecordId\" IS NOT NULL ORDER BY \"EndOfServiceTime\" DESC LIMIT @limit";
            return await _context.SqlQueryAsync<ServiceRecordsDto>(sql, new { owner, identityId, limit });
        }

        public async Task DeleteAndSaveChangeAsync(string userId, long id)
        {
            var record = await FindTask(userId, id);
            if (record != null)
            {
                // 软删除
                record.Deleted = true;
                _context.ServiceRecords.Update(record);
                await _context.SaveChangesAsync();
            }
        }
    }
}
