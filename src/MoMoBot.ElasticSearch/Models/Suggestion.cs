using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.ElasticSearch.Models
{
    public class Suggestion
    {
        public string Text { get; set; }
        public string Id { get; set; }
        public string Intent { get; set; }
        public double? Score { get; internal set; }
    }
}
