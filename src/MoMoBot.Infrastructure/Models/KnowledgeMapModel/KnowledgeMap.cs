using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Infrastructure.Models.KnowledgeMapModel
{
    /// <summary>
    /// 知识图谱
    /// </summary>
    [Serializable]
    public class KnowledgeMap
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Identifier { get; set; }
        public string Remark { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
