using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.Hubs
{
    /// <summary>
    /// 客户
    /// </summary>
    public class Customer
    {
        public long Id { get; set; }
        public string IdentityId { get; set; }
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public string ServiceUserId { get; set; }
        public bool Waiting { get; set; } = false;
        public string From { get; set; } = "DTalk";
        public string ChatRoom { get; set; }

        public Dictionary<string, string> ToDictionary() => new Dictionary<string, string> {
            {"IdentityId", IdentityId},
            {"ConnectionId", ConnectionId},
            {"Name", ServiceUserId},
            {"From", From},
            {"ChatRoom", ChatRoom }
        };
    }
}
