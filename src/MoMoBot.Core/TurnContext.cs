using System;
using System.Collections.Generic;
using System.Text;
using MoMoBot.Core.Schema;

namespace MoMoBot.Core
{
    public class TurnContext : IDisposable
    {
        public Activity Activity { get; }
        public TurnContextStateCollection TurnState { get; } = new TurnContextStateCollection();

        public TurnContext(Activity activity)
        {
            Activity = activity;
        }

        public void Dispose()
        {
            TurnState.Dispose();
        }
    }
}
