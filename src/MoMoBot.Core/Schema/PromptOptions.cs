using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Core.Schema
{
    public class PromptOptions
    {
        /// <summary>
        /// Gets or sets the initial prompt to send the user as <seealso cref="Activity"/>Activity.
        /// </summary>
        /// <value>
        /// The initial prompt to send the user as <seealso cref="Activity"/>Activity.
        /// </value>
        public Activity Prompt { get; set; }

        /// <summary>
        /// Gets or sets the retry prompt to send the user as <seealso cref="Activity"/>Activity.
        /// </summary>
        /// <value>
        /// The retry prompt to send the user as <seealso cref="Activity"/>Activity.
        /// </value>
        public Activity RetryPrompt { get; set; }

        //public IList<Choice> Choices { get; set; }

        public object Validations { get; set; }
    }
}
