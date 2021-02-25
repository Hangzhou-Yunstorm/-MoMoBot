using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MoMoBot.Infrastructure.ViewModels;

namespace MoMoBot.Infrastructure.Models
{
    public class ChatRecord
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime Time { get; set; }
        [Required]
        public int Who { get; set; }
        [DefaultValue(false)]
        [Required]
        public bool Unread { get; set; }
        [StringLength(1024)]
        public string Data { get; set; }

        [DefaultValue(MessageTypes.Text)]
        public MessageTypes Type { get; set; }

        public virtual Chat Chat { get; set; }
    }
}
