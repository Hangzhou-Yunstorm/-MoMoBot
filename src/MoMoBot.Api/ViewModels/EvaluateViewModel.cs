using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.ViewModels
{
    public class EvaluateViewModel
    {
        public long RecordId { get; set; }
        public int Score { get; set; }
        public string Content { get; set; }
    }
}
