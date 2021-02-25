using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Infrastructure.Models
{
    public class QueryParameter
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 参数名称
        /// </summary>
        [StringLength(100)]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        [DefaultValue(true)]
        public bool Enable { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [StringLength(int.MaxValue)]
        public string Remarks { get; set; }

        public ICollection<AnswerQueries> AnswerParameters { get; set; }
    }
}
