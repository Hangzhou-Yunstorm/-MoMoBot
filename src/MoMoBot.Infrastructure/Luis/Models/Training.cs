using Newtonsoft.Json;
using System;

namespace MoMoBot.Infrastructure.Luis.Models
{
    public class Training
    {
        public string ModelId { get; set; }

        [JsonProperty("details")]
        public TrainingDetails Details { get; set; }
    }

    public class TrainingDetails
    {
        public int StatusId { get; set; }
        public string Status { get; set; }
        public int ExampleCount { get; set; }
        public DateTime? TrainingDateTime { get; set; }
        public string FailureReason { get; set; }
    }
}
