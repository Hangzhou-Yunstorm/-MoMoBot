using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.ViewModels
{
    public class InputSettingViewModel
    {
        [Required]
        [MaxLength(100)]
        public string Key { get; set; }
        [Required]
        [MaxLength(200)]
        public string Value { get; set; }
    }
}
