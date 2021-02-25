using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MoMoBot.Infrastructure.Enums;
using MoMoBot.Infrastructure.Models;
using MoMoBot.Infrastructure.ViewModels;
using MoMoBot.Service.Implements;
using NPOI.HSSF.Record.Chart;

namespace MoMoBot.Service.Abstractions
{
    public interface IChatService
    {
        /// <summary>
        /// 新建或者更新记录
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="customerId"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        Task<(string groupName, long id)> CreateOrUpdateRecord(string serviceId, string customerId, string title);
        /// <summary>
        /// 设置为离线
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        Task SetOffline(string customerId);
        /// <summary>
        /// 获取所有聊天室
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        Task<List<string>> GetGroups(string serviceId);
        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns></returns>

        Task<int> SaveChangesAsync();

        Task<Chat> FindFirstAsync(string serviceId, string customerId);
        Task<Chat> FindFirstAsync(Expression<Func<Chat,bool>> where);
        /// <summary>
        /// 获取聊天联系人列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<Contact>> GetContactsAsync(string userId);
        /// <summary>
        /// 接收到消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="sender">发送者id，为空则为系统</param>
        /// <param name="unread">是否未读</param>
        /// <param name="save">是否立即保存</param>
        /// <returns></returns>
        Task ReceiveNewMessage(Message message, string sender = "", bool unread = true, bool save = true);
        /// <summary>
        /// 获取聊天消息记录
        /// </summary>
        /// <param name="chatId">对话id</param>
        /// <param name="offset">跳过</param>
        /// <param name="limit">取</param>
        /// <returns></returns>
        Task<PaginationViewModel> GetRecordsAsync(long chatId, int offset = 0, int limit = 50);
        /// <summary>
        /// 设置所有人为离线
        /// </summary>
        /// <returns></returns>
        Task OfflineEveryOneAsync();
        Task<List<Chat>> FindAsync(Func<Chat, bool> p);
        
    }
}
