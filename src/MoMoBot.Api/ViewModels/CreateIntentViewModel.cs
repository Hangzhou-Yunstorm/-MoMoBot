using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.ViewModels
{
    public class CreateIntentViewModel
    {
        [Required]
        public string Intent { get; set; }
    }
}
