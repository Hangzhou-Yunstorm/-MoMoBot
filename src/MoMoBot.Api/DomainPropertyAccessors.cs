using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MoMoBot.Core;

namespace MoMoBot.Api
{
    public class DomainPropertyAccessors
    {
        private readonly ConversationState _conversationState;
        public DomainPropertyAccessors(ConversationState conversationState)
        {
            _conversationState = conversationState;

            TestModelPropertyAccessor = _conversationState.CreateProperty<TestModel>(TestPropertyName);
            BusinessEnquiryPropertyAccessor = _conversationState.CreateProperty<BusinessEnquiry>(BusinessEnquiryPropertyName);
            DialogStateProperty = conversationState.CreateProperty<DialogState>(DialogStatePropertyName);
        }
        private const string DialogStatePropertyName = "momobot.dialogstate";
        public IStatePropertyAccessor<DialogState> DialogStateProperty { get; private set; }
        private const string TestPropertyName = "momobot.testmodel";
        public IStatePropertyAccessor<TestModel> TestModelPropertyAccessor { get; private set; }

        private const string BusinessEnquiryPropertyName = "momobot.businessenquiry";
        public IStatePropertyAccessor<BusinessEnquiry> BusinessEnquiryPropertyAccessor { get; private set; }

        public async Task SaveStatesAsync(TurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }
    }

    public class TestModel
    {
        public string Id { get; set; } = string.Empty;
    }

    public class BusinessEnquiry
    {
        public string SheetName { get; set; }
        public string ColumnName { get; set; }
    }
}
