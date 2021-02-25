using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.Hubs
{
    /// <summary>
    /// 客服人员
    /// </summary>
    public class CustomerService
    {
        public string ConnectionId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public Dictionary<string, string> ToDictionary() => new Dictionary<string, string> {
            { "UserId",UserId},
            { "Name",Name},
            { "Email",Email},
        };
    }
}
