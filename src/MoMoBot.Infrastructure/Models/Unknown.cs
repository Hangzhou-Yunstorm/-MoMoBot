using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Infrastructure.Models
{
    public class Unknown
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 发生时间
        /// </summary>
        [Required]
        public DateTime TimeOfOccurrence { get; set; }

        public string Remarks { get; set; }

        /// <summary>
        /// 类型（1：未知问题 2：无解意图 3：其他）
        /// </summary>
        [DefaultValue(1)]
        public int Type { get; set; }

        /// <summary>
        /// 是否已解决
        /// </summary>
        [DefaultValue(false)]
        public bool IsResolved { get; set; }
    }
}
