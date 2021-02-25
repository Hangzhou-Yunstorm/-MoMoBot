using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.ViewModels
{
    public class UpdateIntentViewModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Intent { get; set; }
        public bool UpdatePermission { get; set; } = false;
        public long[] Permissions { get; set; }

    }
}
