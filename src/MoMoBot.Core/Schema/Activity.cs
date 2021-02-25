using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Core.Schema
{
    public class Activity
    {
        public string ChannelId { get; set; }
        public Conversation Conversation { get; set; }
        public string Type { get; internal set; }

        public object Value { get; set; }
    }
}
