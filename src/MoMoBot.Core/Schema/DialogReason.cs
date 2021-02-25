using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Core.Schema
{
    public enum DialogReason
    {
        /// <summary>
        /// A dialog is being started through a call to `DialogContext.BeginAsync()`.
        /// </summary>
        BeginCalled,

        /// <summary>
        /// A dialog is being continued through a call to `DialogContext.ContinueDialogAsync()`.
        /// </summary>
        ContinueCalled,

        /// <summary>
        /// A dialog ended normally through a call to `DialogContext.EndDialogAsync()`.
        /// </summary>
        EndCalled,

        /// <summary>
        /// A dialog is ending because it's being replaced through a call to `DialogContext.ReplaceDialogAsync()`.
        /// </summary>
        ReplaceCalled,

        /// <summary>
        /// A dialog was cancelled as part of a call to `DialogContext.CancelAllDialogsAsync()`.
        /// </summary>
        CancelCalled,

        /// <summary>
        /// A step was advanced through a call to `WaterfallStepContext.NextAsync()`.
        /// </summary>
        NextCalled,
    }
}
