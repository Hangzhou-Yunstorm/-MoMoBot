using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoMoBot.Core
{
    public interface IBot
    {
        Task OnTurnAsync(TurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken));
    }
}
