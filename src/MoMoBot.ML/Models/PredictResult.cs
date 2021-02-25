using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.ML.Models
{
    public class PredictResult
    {
        public PredictResult(string question, string intent, float score)
        {
            Question = question;
            Intent = intent;
            Score = score;
            PredictIntents = new List<PredictIntent>();
        }

        public string Intent { get; set; }
        public float Score { get; set; }
        public string Question { get; set; }
        public IList<PredictIntent> PredictIntents { get; set; }

        public void AddPredict(string intent, float score)
        {
            PredictIntents.Add(new PredictIntent { Intent = intent, Score = score });
        }
    }

    public class PredictIntent
    {
        public string Intent { get; set; }
        public float Score { get; set; }
    }
}
