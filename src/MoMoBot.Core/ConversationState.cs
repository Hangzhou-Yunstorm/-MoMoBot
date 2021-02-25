using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Core
{
    public class ConversationState : BotState
    {
        public ConversationState(IStorage storage)
            : base(storage, nameof(ConversationState))
        {
        }

        protected override string GetStorageKey(TurnContext turnContext)
        {
            var channelId = turnContext.Activity.ChannelId ?? throw new ArgumentNullException("invalid activity-missing channelId");
            var conversationId = turnContext.Activity.Conversation?.Id ?? throw new ArgumentNullException("invalid activity-missing Conversation.Id");
            return $"{channelId}/conversations/{conversationId}";
        }
    }
}
