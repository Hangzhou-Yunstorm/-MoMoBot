using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MoMoBot.Infrastructure.Models
{
    public class Voice
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(256)]
        public string SavePath { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

        [DefaultValue(0d)]
        public double Duration { get; set; }

        [DefaultValue("wav")]
        public string AudioType { get; set; }
        [StringLength(255)]
        public string Text { get; set; }
    }
}
