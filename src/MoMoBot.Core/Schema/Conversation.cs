using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Core.Schema
{
    public class Conversation
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public int Type { get; set; }
    }
}
