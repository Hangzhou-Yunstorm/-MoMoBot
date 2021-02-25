using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MoMoBot.Infrastructure.Extensions;
using MoMoBot.Infrastructure.Models;
using MoMoBot.Infrastructure.ViewModels;
using MoMoBot.Service.Abstractions;
using Newtonsoft.Json;

namespace MoMoBot.Api.Hubs
{
    /// <summary>
    /// 人工服务hub
    /// </summary>
    public class ArtificialServicesHub
        : Hub
    {
        #region 注入
        private readonly ILogger<ArtificialServicesHub> _logger;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ArtificialServices _artificial;
        private readonly IChatService _chat;
        public ArtificialServicesHub(
            IChatService chat,
            ILogger<ArtificialServicesHub> logger,
            TokenValidationParameters tokenValidationParameters,
            ArtificialServices artificial)
        {
            _chat = chat;
            _logger = logger;
            _tokenValidationParameters = tokenValidationParameters;
            _artificial = artificial;
        } 
        #endregion

        /// <summary>
        /// 客服人员上线
        /// </summary>
        public async Task CustomerServiceOnline()
        {
            var cs = GetCurrentUserInfo();

            if (!string.IsNullOrEmpty(cs.UserId))
            {
                await _artificial.CustomerServiceOnlineAsync(cs);
                var customers = await _artificial.GetAllWaitingCustomersAsync();
                if (customers.Keys.Count > 0)
                {
                    foreach (var key in customers.Keys)
                    {
                        var customer = JsonConvert.DeserializeObject<Customer>(customers[key]);
                        var chatRoom = await _artificial.CreateOrUpdateChatRoomAsync(cs.UserId, customer);
                        customer.ChatRoom = chatRoom.groupName;
                        customer.ServiceUserId = cs.UserId;
                        customer.Id = chatRoom.id;
                        await Clients.Client(cs.ConnectionId).SendAsync("newCustomerJoined", customer);
                        await Clients.Client(customer.ConnectionId).SendAsync("startService", new { cs.Name, chatRoom.groupName });
                        await _artificial.CustomerOnlineAsync(customer);
                        await _artificial.RemoveWaitingCustomerAsync(key);
                    }
                }
            }
        }

        /// <summary>
        /// 新客户加入
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public async Task NewCustomer(Customer customer)
        {
            customer.ConnectionId = Context.ConnectionId;
            var cs = await _artificial.GetOneOnlineCustomerServiceAsync();
            // 加入对话
            if (string.IsNullOrWhiteSpace(cs) == false)
            {
                await _artificial.CustomerOnlineAsync(customer);
                var chatRoom = await _artificial.CreateOrUpdateChatRoomAsync(cs, customer);
                customer.ChatRoom = chatRoom.groupName;
                customer.ServiceUserId = cs;
                customer.Id = chatRoom.id;

                // 给客服人员发送客户已加入的消息，通知服务
                var connectionId = await _artificial.GetOnlineCSConnectionIdByUserIdAsync(cs);
                await Clients.Client(connectionId).SendAsync("newCustomerJoined", customer);
                //await _artificial.SendNoticeAsync("对话已接通", cs);
                var csInfo = await _artificial.GetCustomerServiceInfoAsync(cs);
                // 给客户端发送已经连接到客服的消息，可以进行咨询
                await Clients.Caller.SendAsync("startService", new { csInfo.Name, chatRoom.groupName });
            }
            // 当前没有客服在线，等待
            else
            {
                // TODO : 钉钉通知客服上线

                // 等待客服接入
                await _artificial.AddWaitingCustomerAsync(customer);
                await Clients.Caller.SendAsync("waitService");
                return;
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(Message message)
        {
            if (!message.IsValid())
            {
                throw new ArgumentException(nameof(message));
            }
            message.Time = DateTime.Now;
            // 保存聊天记录
            await _chat.ReceiveNewMessage(message, sender: message.Sender);
            await Clients.OthersInGroup(message.GroupName)
                .SendAsync("receiveMessage", message);
        }

        /// <summary>
        /// 客户挂断
        /// </summary>
        /// <param name="waiting"></param>
        /// <returns></returns>
        public async Task Hangup(bool waiting = false)
        {
            var identityId = GetCustomerIdentityId();
            // 取消等待
            if (waiting)
            {
                await _artificial.CancelWaitingAsync(identityId);
            }
            // 挂断对话
            else
            {
                await _artificial.CustomerOfflineAsync(identityId);
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            var cs = GetCurrentUserInfo();
            // 客服人员
            if (!string.IsNullOrEmpty(cs?.UserId))
            {
                await _artificial.CustomerServiceOfflineAsync(cs);
            }
            else
            {
                await _artificial.CustomerOfflineAsync(GetCustomerIdentityId());
            }
        }

        /// <summary>
        /// 获取客户的识别id
        /// </summary>
        /// <returns></returns>
        private string GetCustomerIdentityId() => Context.GetHttpContext().Request.Query["identityId"];

        /// <summary>
        /// 获取客服的信息
        /// </summary>
        /// <returns></returns>
        private CustomerService GetCurrentUserInfo()
        {
            CustomerService cs = null;

            try
            {
                var userInfo = Context.GetHttpContext().IdentityUser<MoMoBotUser>(_tokenValidationParameters);
                if (userInfo != null)
                {
                    cs = new CustomerService
                    {
                        ConnectionId = Context.ConnectionId,
                        Email = userInfo.Email,
                        Name = userInfo.UserName,
                        UserId = userInfo.Id
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return cs;
        }
    }
}
