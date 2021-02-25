using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MoMoBot.Infrastructure;
using MoMoBot.Infrastructure.Models;
using MoMoBot.Infrastructure.ViewModels;
using MoMoBot.Service.Abstractions;
using Newtonsoft.Json;

namespace MoMoBot.Api.Hubs
{
    public class ArtificialServices
    {
        #region 注入
        private readonly ILogger<ArtificialServices> _logger;
        private readonly IRedisCachingProvider _redis;
        private readonly IHubContext<ArtificialServicesHub> _hub;
        private readonly IChatService _chat;
        private readonly IServiceRecordService _record;
        public ArtificialServices(IRedisCachingProvider redis,
            ILogger<ArtificialServices> logger,
            IServiceRecordService record,
            IChatService chat,
            IHubContext<ArtificialServicesHub> hub)
        {
            _redis = redis;
            _logger = logger;
            _hub = hub;
            _record = record;
            _chat = chat;
        }
        #endregion

        /// <summary>
        /// 客服上线
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        public async Task CustomerServiceOnlineAsync(CustomerService cs)
        {
            await _redis.HSetAsync(Constants.RedisKey.AllOnlineCustomerService, cs.UserId, cs.ConnectionId);
            await _redis.HMSetAsync(cs.UserId, cs.ToDictionary());

            var groups = await _chat.GetGroups(cs.UserId);
            groups?.ForEach(async (g) => { await _hub.Groups.AddToGroupAsync(cs.ConnectionId, g); });
        }

        /// <summary>
        /// 获取所有正在等待的客户
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetAllWaitingCustomersAsync()
        {
            return await _redis.HGetAllAsync(Constants.RedisKey.AllWaitingCustomers);
        }

        /// <summary>
        /// 客户上线
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public async Task CustomerOnlineAsync(Customer customer)
        {
            // 已在线并且connectionid已改变
            if (await _redis.HExistsAsync(Constants.RedisKey.AllOnlineCustomers, customer.IdentityId) &&
                await _redis.HGetAsync(Constants.RedisKey.AllOnlineCustomerService, customer.IdentityId) != customer.ConnectionId)
            {
                // TODO: 通知客服客户connectionid已改变
            }
            await _redis.HSetAsync(Constants.RedisKey.AllOnlineCustomers, customer.IdentityId, customer.ConnectionId);
        }

        /// <summary>
        /// 在等待客服接入列表中删除该客户
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task RemoveWaitingCustomerAsync(string key)
        {
            await _redis.HDelAsync(Constants.RedisKey.AllWaitingCustomers, new List<string> { key });
        }

        /// <summary>
        /// 客服下线
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        public async Task CustomerServiceOfflineAsync(CustomerService cs)
        {
            var connectionId = await _redis.HGetAsync(Constants.RedisKey.AllOnlineCustomerService, cs.UserId);
            if (string.IsNullOrWhiteSpace(connectionId) == false)
            {
                var groups = await _chat.GetGroups(cs.UserId);
                groups?.ForEach(async (group) => { await _hub.Groups.RemoveFromGroupAsync(connectionId, group); });
            }
            await _redis.HDelAsync(Constants.RedisKey.AllOnlineCustomerService, new List<string> { cs.UserId });
        }

        /// <summary>
        /// 客户下线
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public async Task CustomerOfflineAsync(string customerId)
        {
            var connectionId = await _redis.HGetAsync(Constants.RedisKey.AllOnlineCustomers, customerId);
            await _redis.HDelAsync(Constants.RedisKey.AllOnlineCustomers, new List<string> { customerId });
            // 通知客服
            var records = await _chat.FindAsync(c => c.Other == customerId && c.Online);
            await _chat.SetOffline(customerId);
            await _chat.SaveChangesAsync();
            if (!string.IsNullOrEmpty(connectionId))
            {
                if (records != null)
                {
                    foreach (var record in records)
                    {
                        // 生成一条归档记录
                        var sr = await _record.GenerateServiceRecordAsync(record.Owner, record.Id);
                        if (sr != null)
                        {
                            record.ServiceRecord = sr;
                            record.Online = false;
                            await _chat.SaveChangesAsync();
                        }
                        await _hub.Clients.Client(connectionId).SendAsync("serviceScore", record.Id);
                        var server = await _redis.HGetAsync(Constants.RedisKey.AllOnlineCustomerService, record.Owner);
                        if (!string.IsNullOrEmpty(server))
                        {
                            await _hub.Groups.RemoveFromGroupAsync(server, record.GroupName);
                            await _hub.Clients.Client(server).SendAsync("customerOffline", record.GroupName);
                        }
                        await _hub.Groups.RemoveFromGroupAsync(connectionId, record.GroupName);
                    }
                }
            }
        }

        /// <summary>
        /// 取消等待客服接入
        /// </summary>
        /// <param name="identityId"></param>
        /// <returns></returns>
        public async Task CancelWaitingAsync(string identityId)
        {
            var json = await _redis.HGetAsync(Constants.RedisKey.AllWaitingCustomers, identityId);
            var customer = JsonConvert.DeserializeObject<Customer>(json);
            if (!string.IsNullOrEmpty(customer.ConnectionId))
            {
                await _hub.Clients.Client(customer.ConnectionId).SendAsync("cancelWaiting");
            }
            await _redis.HDelAsync(Constants.RedisKey.AllWaitingCustomers, new List<string> { identityId });
        }

        /// <summary>
        /// 获取所有在线客服
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetAllOnlineCustomerServicesAsync()
        {
            return await _redis.HGetAllAsync(Constants.RedisKey.AllOnlineCustomerService);
        }

        /// <summary>
        /// 客户进入等待
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public async Task AddWaitingCustomerAsync(Customer customer)
        {
            await _redis.HSetAsync(Constants.RedisKey.AllWaitingCustomers, customer.IdentityId, JsonConvert.SerializeObject(customer));
        }


        /// <summary>
        /// 创建聊天室
        /// </summary>
        /// <param name="csUserId"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public async Task<(string groupName, long id)> CreateOrUpdateChatRoomAsync(string csUserId, Customer customer)
        {
            var cs = await GetOnlineCSConnectionIdByUserIdAsync(csUserId);
            if (!string.IsNullOrEmpty(cs))
            {
                var result = await _chat.CreateOrUpdateRecord(csUserId, customer.IdentityId, customer.Name);
                await _chat.SaveChangesAsync();

                await _hub.Groups.AddToGroupAsync(cs, result.groupName);
                await _hub.Groups.AddToGroupAsync(customer.ConnectionId, result.groupName);

                //var message = new Message { Content = "对话已接入", GroupName = result.groupName, Time = DateTime.Now, Type = MessageTypes.Notice };
                //await _chat.ReceiveNewMessage(message, sender: string.Empty);
                await _redis.HMSetAsync(result.groupName, new Dictionary<string, string> { { csUserId, cs }, { customer.IdentityId, customer.ConnectionId }, { "start", DateTime.Now.ToString() } });
                //await _hub.Clients.Client(cs).SendAsync("receiveMessage", message);

                return result;
            }
            return (string.Empty, 0L);
        }

        /// <summary>
        /// 分配一位在线客服
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetOneOnlineCustomerServiceAsync()
        {
            var keys = await _redis.HKeysAsync(Constants.RedisKey.AllOnlineCustomerService);
            if (keys?.Count <= 0)
            {
                return null;
            }
            var count = keys.Count;
            var index = new Random().Next(0, count);
            var key = string.Empty;
            for (var i = 0; i < count; i++)
            {
                if (i == index)
                {
                    key = keys[i];
                    break;
                }
            }
            return key;
        }

        /// <summary>
        /// 应用关闭
        /// </summary>
        /// <returns></returns>
        public async Task ShutdownAsync()
        {
            // TODO : 将所有在线人员全部置为离线
            _logger.LogInformation("应用即将关闭...");
            await _chat.OfflineEveryOneAsync();
            await _redis.HDelAsync(Constants.RedisKey.AllOnlineCustomers);
            await _redis.HDelAsync(Constants.RedisKey.AllWaitingCustomers);
            await _redis.HDelAsync(Constants.RedisKey.AllOnlineCustomerService);
        }

        /// <summary>
        /// 获取客服的信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<CustomerService> GetCustomerServiceInfoAsync(string userId)
        {
            CustomerService cs = null;
            var dic = await _redis.HMGetAsync(userId, new List<string> { "Name", "Email" });
            if (dic.Keys?.Count > 0)
            {
                cs = new CustomerService
                {
                    UserId = userId,
                    Name = dic["Name"],
                    Email = dic["Email"]
                };
            }
            return cs;
        }

        /// <summary>
        /// 通过用户id获取在线客服的connectionid
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<string> GetOnlineCSConnectionIdByUserIdAsync(string userId)
        {
            return await _redis.HGetAsync(Constants.RedisKey.AllOnlineCustomerService, userId);
        }

        /// <summary>
        /// 发送通知消息给客服
        /// </summary>
        /// <param name="content"></param>
        /// <param name="csUserId"></param>
        /// <returns></returns>
        public async Task SendNoticeToCustomerServiceAsync(string content, string csUserId)
        {
            var message = new ChatRecord
            {
                Content = content,
                Type = MessageTypes.Notice,
                Who = 0,
                Time = DateTime.Now
            };
            var server = await _redis.HGetAsync(Constants.RedisKey.AllOnlineCustomerService, csUserId);
            if (!string.IsNullOrEmpty(server))
            {
                await _hub.Clients.Client(server).SendAsync("receiveMessage", message);
            }
        }
    }
}
