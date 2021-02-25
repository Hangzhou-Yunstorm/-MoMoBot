using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MoMoBot.Infrastructure.Models.KnowledgeMapModel
{
    /// <summary>
    /// 图谱步骤
    /// </summary>
    public class MapStep
    {
        public MapStep()
        {
            PrevStep = 0;
            NextStep = 0;
        }

        [Key]
        public long Id { get; set; }

        public long MapId { get; set; }

        public long? PrevStep { get; set; }
        public long? NextStep { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        public StepTypes StepType { get; set; }
        public string Question { get; set; }
        public string Key { get; set; }

        [DefaultValue("String")]
        public string ResultType { get; set; }
        public string TriggeredResult { get; set; }
        /// <summary>
        /// 触发函数
        /// </summary>
        public string Function { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        public double? X { get; set; }
        public double? Y { get; set; }
        public string Label { get; set; }

        [StringLength(100)]
        public string TempId { get; set; }

        [ForeignKey(nameof(MapId))]
        public virtual KnowledgeMap KnowledgeMap { get; set; }
    }

    public enum StepTypes
    {
        StartNode = 1,
        RunningNode = 2,
        BranchNode = 3,
        EndNode = 4
    }
}
