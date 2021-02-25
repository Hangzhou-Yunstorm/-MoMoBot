using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.ViewModels
{
    public class EditFlowViewModel
    {
        public int FlowId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Key { get; set; }
        [MaxLength(256)]
        public string Remark { get; set; }
    }
}
