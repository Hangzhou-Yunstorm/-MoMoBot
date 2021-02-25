using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Infrastructure.Models
{
    public class TopScoringIntent
    {
        /// <summary>
        /// 问候
        /// </summary>
        public string intent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double score { get; set; }
    }

    public class EntitiesItem
    {
        /// <summary>
        /// 你好
        /// </summary>
        public string entity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int startIndex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int endIndex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double score { get; set; }
    }

    public class IntentResult
    {
        /// <summary>
        /// 你好啊？
        /// </summary>
        public string query { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TopScoringIntent topScoringIntent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<EntitiesItem> entities { get; set; }
    }
}
