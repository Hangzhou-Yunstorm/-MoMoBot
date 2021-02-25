using System;

namespace MoMoBot.Infrastructure.Luis.Models
{
    [Serializable]
    public class Intent
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
