using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Core.Schema
{
    /// <summary>
    /// Tracking information for a dialog on the stack.
    /// </summary>
    public class DialogInstance
    {
        /// <summary>
        /// Gets or sets the ID of the dialog this instance is for.
        /// </summary>
        /// <value>
        /// ID of the dialog this instance is for.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the instance's persisted state.
        /// </summary>
        /// <value>
        /// The instance's persisted state.
        /// </value>
        public IDictionary<string, object> State { get; set; }
    }
}
