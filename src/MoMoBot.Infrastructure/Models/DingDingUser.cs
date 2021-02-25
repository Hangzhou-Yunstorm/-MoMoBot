using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Infrastructure.Models
{
    public class DingDingUser
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string[] Departments { get; set; }
        public string[] DepartIds { get; set; }

        public static List<string> GetFields() => new List<string>
        {
            nameof(UserId),
            nameof(UserName),
            nameof(Email),
            nameof(Departments),
            nameof(DepartIds),
        };

        public Dictionary<string, string> ToKeyValues() => new Dictionary<string, string>
        {
            { nameof(UserId), UserId},
            { nameof(UserName), UserName},
            { nameof(Email), Email},
            { nameof(Departments), string.Join(",",Departments)},
            { nameof(DepartIds), string.Join(",",DepartIds)},
        };
    }

    public static class DingDingUserExtensions
    {
        public static DingDingUser GetUserInfoFromDictionary(this Dictionary<string, string> keyValues)
        {
            try
            {
                var user = new DingDingUser();
                user.UserId = keyValues[nameof(DingDingUser.UserId)];
                user.UserName = keyValues[nameof(DingDingUser.UserName)];
                user.Email = keyValues[nameof(DingDingUser.Email)];
                user.Departments = keyValues[nameof(DingDingUser.Departments)]?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                user.DepartIds = keyValues[nameof(DingDingUser.DepartIds)]?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
