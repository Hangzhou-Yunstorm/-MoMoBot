using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoMoBot.Core
{
    public class BotStateSet
    {
        private List<BotState> BotStates = new List<BotState>();
        public BotStateSet(params BotState[] botStates)
        {
            BotStates.AddRange(botStates);
        }

        public void Add(BotState botState)
        {
            MoMoBotAssert.NotNull(botState);
            BotStates.Add(botState);
        }

        public async Task LoadAllAsync(TurnContext turnContext, bool force = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tasks = BotStates.Select(bs => bs.LoadAsync(turnContext, force, cancellationToken)).ToList();
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public async Task SaveAllChangesAsync(TurnContext turnContext, bool force = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tasks = this.BotStates.Select(bs => bs.SaveChangesAsync(turnContext, force, cancellationToken)).ToList();
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
