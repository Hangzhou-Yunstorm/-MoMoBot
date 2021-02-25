using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.ViewModels
{
    public class ServiceRecordModel
    {
        public ServiceRecordModel()
        {
            IsDone = false;
        }
        [Required]
        public string Title { get; set; }

        public long Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Remarks { get; set; }

        public bool IsDone { get; set; }
    }
}
