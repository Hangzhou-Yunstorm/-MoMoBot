using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MoMoBot.Api.ViewModels;
using MoMoBot.Infrastructure.DTalk;
using MoMoBot.Infrastructure.DTalk.Models;
using MoMoBot.Infrastructure.Extensions;
using MoMoBot.Infrastructure.Models;
using MoMoBot.Service.Abstractions;

namespace MoMoBot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        #region 服务注入
        private readonly TokenValidationParameters _validationParameters;
        private readonly IChatService _chat;
        private readonly DTalkConfig _ddConfig;
        private readonly IRedisCachingProvider _redis;
        private readonly IServiceRecordService _record;
        private readonly DingTalkHelper _ddHelper;
        public ChatController(TokenValidationParameters validationParameters,
            IChatService chat,
            DingTalkHelper ddHelper,
            IRedisCachingProvider redis,
            IServiceRecordService record,
        IOptions<DTalkConfig> options)
        {
            _ddHelper = ddHelper;
            _record = record;
            _redis = redis;
            _validationParameters = validationParameters;
            _chat = chat;
            _ddConfig = options.Value ?? throw new ArgumentNullException(nameof(_ddConfig));
        } 
        #endregion

        [HttpGet("list")]
        public async Task<IActionResult> GetRecordList()
        {
            var current = HttpContext.IdentityUser<MoMoBotUser>(_validationParameters);
            if (current != null)
            {
                var list = await _chat.GetContactsAsync(current.Id);
                return Ok(list);
            }
            return Unauthorized();
        }

        [HttpGet("records")]
        public async Task<IActionResult> GetRecords(long chatId, int pageIndex = 1, int pageSize = 50)
        {
            var offset = (pageIndex - 1) * pageSize;
            var result = await _chat.GetRecordsAsync(chatId, offset, pageSize);
            return Ok(result);
        }

        [HttpPost("evaluate")]
        public async Task<IActionResult> Evaluate([FromBody]EvaluateViewModel vm)
        {
            await _record.ScoreAsync(vm.RecordId, vm.Score);
            return Ok();
        }

        [HttpGet("customerinfo")]
        public async Task<IActionResult> GetCustomerInfo(string identityId, string from = "dtalk")
        {

            CustomerInfo customer = null;
            if (string.IsNullOrWhiteSpace(identityId) == false)
            {
                var key = $"customer{identityId}";
                var keyValuePairs = await _redis.HMGetAsync(key, CustomerInfo.GetFields());
                customer = KeyValueToCustomerInfo(keyValuePairs);
                if (string.IsNullOrWhiteSpace(customer.IdentityId))
                {
                    if (from.Equals("dtalk", StringComparison.CurrentCultureIgnoreCase))
                    {
                        customer = await GetDDUserInfoAsync(identityId);
                        if (customer != null)
                        {
                            await _redis.HMSetAsync(key, customer.GetKeyValuePairs());
                        }
                    }
                }
            }

            return Ok(customer);
        }

        [HttpGet("service-records")]
        public async Task<IActionResult> GetServiceRecords(string identityId)
        {
            var current = HttpContext.IdentityUser<MoMoBotUser>(_validationParameters);
            if (current != null)
            {
                var records = await _record.GetServiceRecordsAsync(current.Id, identityId);

                return Ok(records);
            }
            return Unauthorized();
        }

        private CustomerInfo KeyValueToCustomerInfo(Dictionary<string, string> keyValuePairs)
        {
            CustomerInfo customer = new CustomerInfo();
            if (keyValuePairs.ContainsKey(nameof(CustomerInfo.IdentityId)))
            {
                customer.IdentityId = keyValuePairs[nameof(CustomerInfo.IdentityId)];
            }
            if (keyValuePairs.ContainsKey(nameof(CustomerInfo.Mobile)))
            {
                customer.Mobile = keyValuePairs[nameof(CustomerInfo.Mobile)];
            }
            if (keyValuePairs.ContainsKey(nameof(CustomerInfo.Name)))
            {
                customer.Name = keyValuePairs[nameof(CustomerInfo.Name)];
            }
            if (keyValuePairs.ContainsKey(nameof(CustomerInfo.Avatar)))
            {
                customer.Avatar = keyValuePairs[nameof(CustomerInfo.Avatar)];
            }
            if (keyValuePairs.ContainsKey(nameof(CustomerInfo.Department)))
            {
                customer.Department = keyValuePairs[nameof(CustomerInfo.Department)];
            }
            if (keyValuePairs.ContainsKey(nameof(CustomerInfo.Email)))
            {
                customer.Email = keyValuePairs[nameof(CustomerInfo.Email)];
            }
            if (keyValuePairs.ContainsKey(nameof(CustomerInfo.From)))
            {
                customer.From = keyValuePairs[nameof(CustomerInfo.From)];
            }
            if (keyValuePairs.ContainsKey(nameof(CustomerInfo.Position)))
            {
                customer.Position = keyValuePairs[nameof(CustomerInfo.Position)];
            }
            return customer;
        }

        private async Task<CustomerInfo> GetDDUserInfoAsync(string identityId)
        {
            CustomerInfo customer = null;
            var token = await _ddHelper.GetAccessTokenAsync();
            var user = await _ddHelper.GetUserInfoAsync(token, identityId);
            if (user.errcode == "0")
            {

                customer = new CustomerInfo
                {
                    Avatar = user.avatar,
                    Name = user.name,
                    IdentityId = user.userid,
                    From = "钉钉用户",
                    Email = user.email,
                    Mobile = user.mobile,
                    Position = user.position
                };
                var departmentId = user.department?.FirstOrDefault();
                if (string.IsNullOrEmpty(departmentId) == false)
                {
                    var depart = await _ddHelper.GetDepartmentInfoByIdAsync(token, departmentId);
                    if (depart != null)
                    {
                        customer.Department = depart.Name;
                    }
                }
            }
            return customer;
        }
    }

    public class CustomerInfo
    {
        public string Avatar { get; set; }
        public string Name { get; set; }
        public string From { get; set; }
        public string IdentityId { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }

        public Dictionary<string, string> GetKeyValuePairs() => new Dictionary<string, string> {
            {"IdentityId",IdentityId },
            {"Name",Name },
            {"Email",Email },
            {"Mobile",Mobile },
            {"Department",Department },
            {"Avatar",Avatar },
            {"From",From },
            {"Position",Position },
        };

        public static IList<string> GetFields() => new List<string> {
            nameof(Avatar),
            nameof(Name),
            nameof(From),
            nameof(IdentityId),
            nameof(Email),
            nameof(Department),
            nameof(Mobile),
            nameof(Position),
        };
    }
}
