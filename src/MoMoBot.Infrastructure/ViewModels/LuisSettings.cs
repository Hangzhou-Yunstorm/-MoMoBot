using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Infrastructure.ViewModels
{
    public class LuisSettings
    {
        public Guid AppId { get; set; }
        public string SubscriptionKey { get; set; }
        public string AuthoringKey { get; set; }
        public string LuisVersion { get; set; }
        public string AuthoringEndpoint { get; set; }
        public string RuntimeEndpoint { get; set; }

        public double MinimumMatchingDegree { get; set; }
    }
}
