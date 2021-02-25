using MoMoBot.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Service.Dtos
{
    public class DialogDto
    {
        public string ConversationId { get; set; }
        public Guid Id { get; set; }
        public string Intent { get; set; }
        public string Answer { get; set; }
        public AnswerTypes AnswerType { get; set; }

        public DialogDto(string conversationId, string answer) : this(conversationId, Guid.Empty, "None", answer)
        {
        }

        public DialogDto(string conversationId, Guid answerId, string intent, string answer, AnswerTypes answerType = AnswerTypes.Text)
        {
            ConversationId = conversationId;
            Id = answerId;
            Intent = intent;
            Answer = answer;
            AnswerType = answerType;
        }
    }
}
