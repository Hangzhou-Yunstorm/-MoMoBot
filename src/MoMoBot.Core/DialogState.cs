using System;
using System.Collections.Generic;
using System.Text;
using MoMoBot.Core.Schema;

namespace MoMoBot.Core
{
    public class DialogState
    {
        public DialogState()
            : this(null)
        {
        }

        public DialogState(List<DialogInstance> stack)
        {
            DialogStack = stack ?? new List<DialogInstance>();
        }

        public List<DialogInstance> DialogStack { get; }
    }
}
