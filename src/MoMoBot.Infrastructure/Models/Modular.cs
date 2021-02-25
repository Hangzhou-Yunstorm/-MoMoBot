using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MoMoBot.Infrastructure.Models
{
    public class Modular
    {
        public Modular()
        {
            IsPublic = true;
        }

        [Key]
        public long Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string ModularId { get; set; }
        [Required()]
        [MaxLength(100)]
        public string ModularName { get; set; }
        [Required]
        [DefaultValue(true)]
        public bool IsPublic { get; set; }
        [MaxLength(256)]
        public string Remarks { get; set; }
    }
}
