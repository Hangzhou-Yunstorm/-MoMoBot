using System;

namespace MoMoBot.Infrastructure.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary> 
        /// 获取时间戳 
        /// </summary> 
        /// <returns></returns> 
        public static long GetTimeStamp(this DateTime dateTime)
        {
            TimeSpan ts = dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (long)ts.TotalSeconds;
        }
    }
}
