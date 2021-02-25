using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Infrastructure.ViewModels
{
    /// <summary>
    /// 反馈统计
    /// </summary>
    public class FeedbackStatisticsResult
    {
        public int Count { get; set; }
        public string Intent { get; set; }
    }
}
