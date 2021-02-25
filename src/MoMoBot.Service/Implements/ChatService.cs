using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions;
using MoMoBot.Infrastructure.Database;
using MoMoBot.Infrastructure.Enums;
using MoMoBot.Infrastructure.Models;
using MoMoBot.Infrastructure.ViewModels;
using MoMoBot.Service.Abstractions;
using NPOI.HSSF.Record.Chart;

namespace MoMoBot.Service.Implements
{
    public class ChatService
        : IChatService
    {
        private readonly MoMoDbContext _dbContext;
        public ChatService(MoMoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(string groupName, long id)> CreateOrUpdateRecord(string serviceId, string customerId, string title)
        {
            var groupName = $"group_{Guid.NewGuid()}";
            var chat = await FindFirstAsync(c => c.Owner == serviceId && c.Other == customerId && c.ServiceRecordId.HasValue == false);
            if (chat != null)
            {
                groupName = chat.GroupName;
                chat.DisplayInList = true;
                chat.Online = true;
                _dbContext.Update(chat);
            }
            else
            {
                chat = new Chat
                {
                    UpdateTime = DateTime.Now,
                    Other = customerId,
                    Owner = serviceId,
                    Title = title,
                    GroupName = groupName,
                    Online = true,
                    DisplayInList = true
                };
                await _dbContext.Chats.AddAsync(chat);
                await _dbContext.SaveChangesAsync();
            }
            return (groupName, chat.Id);
        }

        public async Task SetOffline(string customerId)
        {
            var chats = _dbContext.Chats.Where(c => c.Other == customerId && c.Online);
            await chats.ForEachAsync(c => c.Online = false);
            _dbContext.Chats.UpdateRange(chats);
        }

        public async Task<List<string>> GetGroups(string serviceId)
        {
            var groups = _dbContext.Chats.Where(c => c.Online && c.Owner.Equals(serviceId)).Select(c => c.GroupName);
            return await groups.ToListAsync();
        }

        public async Task<bool> ExistedAsync(string serviceId, string customerId)
        {
            return (await FindFirstAsync(serviceId, customerId)) != null;
        }

        public async Task<Chat> FindFirstAsync(string serviceId, string customerId)
        {
            return await _dbContext.Chats.FirstOrDefaultAsync(c => c.Owner == serviceId && c.Other == customerId);
        }

        public async Task<Chat> FindFirstAsync(string groupName)
        {
            return await _dbContext.Chats.FirstOrDefaultAsync(c => c.GroupName == groupName);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Contact>> GetContactsAsync(string userId)
        {
            var sql = "SELECT \"Id\", \"Online\", \"Other\" AS \"Uid\", \"Title\" as \"Name\", COALESCE((SELECT \"Time\" FROM \"ChatRecords\" WHERE \"ChatId\" = a.\"Id\" ORDER BY \"Time\" DESC LIMIT 1),\"UpdateTime\") AS \"Time\",\"GroupName\", COALESCE((SELECT \"Unread\" FROM \"ChatRecords\" WHERE \"ChatId\" = a.\"Id\" ORDER BY \"Unread\" DESC LIMIT 1), FALSE) AS \"Unread\", COALESCE((SELECT \"Content\" FROM \"ChatRecords\" WHERE \"ChatId\" = a.\"Id\" ORDER BY \"Time\" DESC LIMIT 1),'') AS \"Message\" FROM \"Chats\" AS a WHERE \"Owner\" = @userId AND \"DisplayInList\" AND \"ServiceRecordId\" IS NUll ORDER BY \"Online\" DESC, \"Time\" DESC";
            var list = await _dbContext.SqlQueryAsync<Contact>(sql, new { userId });
            return list.Where(item => item.Time.HasValue);
        }

        public async Task ReceiveNewMessage(Message message, string sender = "", bool unread = true, bool save = true)
        {
            var chat = await FindFirstAsync(c => c.GroupName == message.GroupName && c.Online);
            if (chat != null)
            {
                var role = MessageRoles.System;
                if (!string.IsNullOrWhiteSpace(message.Sender))
                {
                    role = message.Sender == chat.Owner ? MessageRoles.Self : MessageRoles.Other;
                }

                var record = new ChatRecord
                {
                    Content = message.Content,
                    Data = message.Data?.ToString(),
                    Time = message.Time,
                    Unread = unread,
                    Type = message.Type,
                    Who = (int)role,
                    Chat = chat
                };
                await _dbContext.ChatRecords.AddAsync(record);
                if (save)
                {
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<PaginationViewModel> GetRecordsAsync(long chatId, int offset = 0, int limit = 50)
        {
            var count = await _dbContext.ChatRecords.CountAsync(c => c.Chat.Id == chatId);

            var records = (from record in _dbContext.ChatRecords
                           where record.Chat.Id == chatId
                           orderby record.Time descending
                           select record)
                            .Skip(offset)
                            .Take(limit)
                            .OrderBy(r => r.Time);
            return new PaginationViewModel
            {
                Rows = records,
                Total = count
            };
        }

        public async Task OfflineEveryOneAsync()
        {
            var online = _dbContext.Chats.Where(c => c.Online);
            await online.ForEachAsync(o => o.Online = false);
            _dbContext.Chats.UpdateRange(online);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Chat> FindFirstAsync(Expression<Func<Chat, bool>> where)
        {
            return await _dbContext.Chats.FirstOrDefaultAsync(where);
        }

        public async Task<List<Chat>> FindAsync(Func<Chat, bool> where)
        {
            return await Task.FromResult(_dbContext.Chats.Where(where).ToList());
        }


    }

    public class Contact
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime? Time { get; set; }
        public string Message { get; set; }
        public bool Unread { get; set; }
        public bool Online { get; set; }
        public string GroupName { get; set; }
        public string Uid { get; set; }
        public string From { get; set; }
    }
}
