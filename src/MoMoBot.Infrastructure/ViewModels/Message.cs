using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Infrastructure.ViewModels
{
    public class Message
    {
        public string Sender { get; set; }
        public string GroupName { get; set; }
        public string Content { get; set; }
        public MessageTypes Type { get; set; } = MessageTypes.Text;
        public DateTime Time { get; set; }
        public object Data { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Sender) &&
                !string.IsNullOrWhiteSpace(GroupName) &&
                !string.IsNullOrWhiteSpace(Content);
        }
    }

    public enum MessageTypes
    {
        Text = 2,
        Image = 4,
        Voice = 8,
        Notice = 16
    }
}
