using MoMoBot.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MoMoBot.Infrastructure.Models
{
    [Serializable]
    public class QandA
    {
        /// <summary>
        /// id
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 答案
        /// </summary>
        [Required]
        [StringLength(int.MaxValue)]
        public string Answer { get; set; }

        /// <summary>
        /// 回复类型
        /// </summary>
        [DefaultValue(AnswerTypes.Text)]
        public AnswerTypes AnswerType { get; set; }

        /// <summary>
        /// 意图
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Intent { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        [StringLength(255)]
        public string RequestUrl { get; set; }

        /// <summary>
        /// 请求方式（1：Get 2：Post）
        /// </summary>
        [DefaultValue(1)]
        public int Method { get; set; }

        /// <summary>
        /// 是否公开
        /// </summary>
        [DefaultValue(true)]
        [Required]
        public bool IsPublic { get; set; }

        /// <summary>
        /// 流程ID（当 <para>AnswerType = AnswerTypes.ProcessFlow</para>）时使用
        /// </summary>
        [StringLength(255)]
        public string FlowId { get; set; }

        /// <summary>
        /// 资源地址（当 <para>AnswerType = AnswerTypes.Voice || AnswerTypes.Video || AnswerTypes.Image</para>）时使用
        /// </summary>
        public string ResourceUrl { get; set; }

        public ICollection<AnswerQueries> AnswerQueries { get; set; }
    }
}
