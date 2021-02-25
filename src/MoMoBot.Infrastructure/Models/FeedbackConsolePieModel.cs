using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Infrastructure.Models
{
    public class FeedbackConsolePieModel
    {
        /// <summary>
        /// 总反馈记录
        /// </summary>
        public int Total{ get;set; }

        /// <summary>
        /// 满意总数
        /// </summary>
        public int SatisfiedCount{ get; set;}

        /// <summary>
        /// 基本解决了问题 总数
        /// </summary>
        public int CommonlyCount { get; set; }

        /// <summary>
        /// 答非所问总数
        /// </summary>
        public int DissatisfiedCount { get; set; }
    }
}
