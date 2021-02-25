using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Infrastructure.Models
{
    public class FeedbackInfo
    {
        [Key]
        public Guid FBId { get; set; }

        /// <summary>
        /// 提出问题人Id
        /// </summary>
        public string PutQuestionsId { get; set; }

        /// <summary>
        /// 匹配的意图
        /// </summary>
        public string Intent { get; set; }

        /// <summary>
        /// 匹配的意图分数
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// 提出问题内容
        /// </summary>
        public string QuestionContent { get; set; }
        /// <summary>
        /// AI回答问题内容
        /// </summary>
        public string AnswerTextContent { get; set; }

        /// <summary>
        /// 发生时间
        /// </summary>
        [Required]
        public DateTime TimeOfOccurrence { get; set; }

        /// <summary>
        /// 软删除
        /// </summary>
        [DefaultValue(false)]
        public bool IsDelete { get; set; }

        /// <summary>
        /// 反馈结果（1：满意 2：一般 3：糟糕）
        /// </summary>
        public int FeedbackResults { get; set; }
    }
}
