using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Infrastructure
{
    public class Constants
    {
        public static string Secret = "asdgasgsagsdgfhgjgfhjhglkyrturthert";

        public static class CacheKey
        {
            public const string Languages = "Languages";
        }

        public static class RedisKey
        {
            public const string AllIntents = "all_intents";
            /// <summary>
            /// 所有在线客服
            /// </summary>
            public const string AllOnlineCustomerService = "all_online_customer_services";
            /// <summary>
            /// 所有在线客户
            /// </summary>
            public const string AllOnlineCustomers = "all_online_customers";
            /// <summary>
            /// 所有等待中的客户
            /// </summary>
            public const string AllWaitingCustomers = "all_waiting_customers";
            public const string ChatRooms = "chat_rooms";

            public const string MatchIntents = "match_intents";

            public const string Settings = "settings";

            public const string IntentMinimumMatchingDegree = "intent_minimum_matching_degree";
        }
    }
}
