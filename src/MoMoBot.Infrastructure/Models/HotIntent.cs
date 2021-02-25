using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Infrastructure.Models
{
    public class HotIntent
    {
        public HotIntent()
        {
            UpdateTime = DateTime.Now;
        }
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Intent { get; set; }
        [DefaultValue(0)]
        public int Count { get; set; }
        [Required]
        public DateTime UpdateTime { get; set; }
    }
}
