using System.Collections.Generic;

namespace MoMoBot.Infrastructure.Luis.Models
{
    public class Example
    {
        public string Text { get; set; }
        public string IntentName { get; set; }
        public IEnumerable<EntityLabel> EntityLabels { get; set; }
    }

    public class EntityLabel
    {
        public string EntityName { get; set; }
        public int StartCharIndex { get; set; }
        public int EndCharIndex { get; set; }
    }
}
