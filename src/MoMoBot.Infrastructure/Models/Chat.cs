using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MoMoBot.Infrastructure.Models
{
    public class Chat
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string Owner { get; set; }
        [Required]
        public string Other { get; set; }
        [DefaultValue(true)]
        public bool DisplayInList { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime UpdateTime { get; set; }
        [StringLength(200)]
        public string GroupName { get; set; }
        [DefaultValue(false)]
        public bool Online { get; set; }

        public long? ServiceRecordId { get; set; }

        public virtual ICollection<ChatRecord> Records { get; set; }
        [ForeignKey(nameof(ServiceRecordId))]
        public virtual ServiceRecord ServiceRecord { get; set; }
    }
}
