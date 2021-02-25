using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Service.Dtos
{
    public class EdgeDto
    {
        public string Id { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public string Label { get; set; }
    }
}
