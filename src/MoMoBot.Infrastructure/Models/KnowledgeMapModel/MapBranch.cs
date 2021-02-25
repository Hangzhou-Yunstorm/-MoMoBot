using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MoMoBot.Infrastructure.Models.KnowledgeMapModel
{
    /// <summary>
    /// 图谱分支
    /// </summary>
    public class MapBranch
    {
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// 分支父节点id
        /// </summary>
        public long ParentStepId { get; set; }
        /// <summary>
        /// 分支触发后进入的节点id
        /// </summary>
        public long StepId { get; set; }

        [DefaultValue("String")]
        public string ResultType { get; set; }
        public string TriggeredResult { get; set; }
        /// <summary>
        /// 触发函数
        /// </summary>
        public string Function { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }
}
