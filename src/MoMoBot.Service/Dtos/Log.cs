using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Service.Dtos
{
    public class Log
    {
        public string message { get; set; }
        public string exception { get; set; }
        public DateTime timestamp { get; set; }
        public int level { get; set; }
        public string log_event { get; set; }
    }
}
