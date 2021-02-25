using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MoMoBot.Infrastructure.Models
{
    public class Settings
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Key { get; set; }
        [StringLength(100)]
        [Required]
        public string Name { get; set; }
        [StringLength(512)]
        [Required]
        public string Value { get; set; }
        [DefaultValue(false)]
        public bool ReadOnly { get; set; }
    }
}
