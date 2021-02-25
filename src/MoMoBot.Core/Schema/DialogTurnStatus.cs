﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Core.Schema
{
    public enum DialogTurnStatus
    {
        /// <summary>
        /// Indicates that there is currently nothing on the dialog stack.
        /// </summary>
        Empty,

        /// <summary>
        /// Indicates that the dialog on top is waiting for a response from the user.
        /// </summary>
        Waiting,

        /// <summary>
        /// Indicates that the dialog completed successfully, the result is available, and the stack is empty.
        /// </summary>
        Complete,

        /// <summary>
        /// Indicates that the dialog was cancelled and the stack is empty.
        /// </summary>
        Cancelled,
    }
}
