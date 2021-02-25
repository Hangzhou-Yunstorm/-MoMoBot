using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoMoBot.Infrastructure.Models
{
    public class Department
    {
        public Department()
        {
            CreationTime = DateTime.Now;
        }
        [Key]
        public long Id { get; set; }
        [MaxLength(100)]
        public string DepartId { get; set; }
        [MaxLength(100)]
        [Required()]
        public string DepartName { get; set; }
        public long? ParentId { get; set; }
        public DateTime CreationTime { get; set; }

        [ForeignKey("ParentId")]
        public Department Parent { get; set; }
    }
}
