using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Infrastructure.Models
{
    public class PaginationViewModel
    {
        public int Total { get; set; }
        public object Rows { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
