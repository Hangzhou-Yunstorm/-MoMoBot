using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MoMoBot.Infrastructure.Models
{
    public class Metadata
    {
        public Metadata()
        {
            AddingTime = DateTime.Now;
        }

        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(250)]
        public string Question { get; set; }
        [Required]
        [StringLength(100)]
        public string Intent { get; set; }
        public DateTime AddingTime { get; set; }
    }
}
