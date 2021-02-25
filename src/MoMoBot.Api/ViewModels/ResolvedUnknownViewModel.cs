using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.ViewModels
{
    public class ResolvedUnknownViewModel
    {
        public Guid Id { get; set; }
        public string Intent { get; set; } = null;
    }
}
