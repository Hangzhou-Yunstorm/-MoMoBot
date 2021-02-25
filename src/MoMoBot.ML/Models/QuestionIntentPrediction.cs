using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.ML.Models
{
    public class QuestionIntentPrediction
    {
        [ColumnName("PredictedLabel")]
        public string Intent { get; set; }
        public float[] Score { get; set; }
    }
}
