using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.ViewModels
{
    public class LogQueryViewModel
    {
        public List<int> Levels { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
