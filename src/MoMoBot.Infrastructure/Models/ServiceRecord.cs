using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Infrastructure.Models
{
    public class ServiceRecord
    {
        public ServiceRecord()
        {
            score = 5;
            IsDone = false;
            EndOfServiceTime = DateTime.Now;
        }

        [Key]
        public long Id { get; set; }
        /// <summary>
        /// 客服id
        /// </summary>
        [Required]
        [StringLength(64)]
        public string UserId { get; set; }

        //[StringLength(50)]
        //public string Name { get; set; }
        /// <summary>
        /// 是否完成归档
        /// </summary>
        [DefaultValue(false)]
        [Required]
        public bool IsDone { get; set; }
        /// <summary>
        /// 聊天记录文件路径
        /// </summary>
        public string RecordFilePath { get; set; }
        /// <summary>
        /// 服务结束时间
        /// </summary>
        public DateTime EndOfServiceTime { get; set; }
        /// <summary>
        /// 归档完成时间
        /// </summary>
        public DateTime RevordCompletionTime { get; set; }
        private int score;
        /// <summary>
        /// 客户打分
        /// </summary>
        [DefaultValue(0)]
        [Required]
        public int Score
        {
            get
            {
                return score;
            }
            set
            {
                if (value < 0)
                {
                    score = 0;
                }
                else if (value > 5)
                {
                    score = 5;
                }
                else
                {
                    score = value;
                }
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        [DefaultValue(false)]
        public bool Deleted { get; set; }

    }
}
