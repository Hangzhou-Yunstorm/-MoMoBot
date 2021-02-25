using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MoMoBot.Api.Dialogs;
using MoMoBot.Core;
using MoMoBot.Core.Schema;

namespace MoMoBot.Api
{
    public class MoMoBot : IBot
    {
        private readonly DialogSet dialogs;
        private readonly DomainPropertyAccessors momoBotAccessors;
        private readonly ILogger<MoMoBot> _logger;
        public MoMoBot(ILogger<MoMoBot> logger,
            DomainPropertyAccessors momoBotAccessors,
            IDialogFactory dialogFactory)
        {
            dialogs = new DialogSet(momoBotAccessors.DialogStateProperty);
            this.momoBotAccessors = momoBotAccessors;
            _logger = logger;

            dialogs.Add(dialogFactory.MainDialog);
        }

        public async Task OnTurnAsync(TurnContext turnContext, CancellationToken cancellationToken = default)
        {
            _logger.LogError("MoMoBot【OnTurnAsync】");

            var dc = await dialogs.CreateContextAsync(turnContext);
            var result = await dc.ContinueDialogAsync();
            if (result.Status == DialogTurnStatus.Empty)
            {
                await dc.BeginDialogAsync(MainDialog.Name);
            }
            await momoBotAccessors.SaveStatesAsync(turnContext, cancellationToken);
        }
    }
}
