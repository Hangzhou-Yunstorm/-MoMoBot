using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MoMoBot.Infrastructure.Models;

namespace MoMoBot.Api.ViewModels
{
    public class InputFeedbackViewModel
    {
        public string UserId { get; set; }
        [Required]
        public string Intent { get; set; }
        public double Score { get; set; }
        [Required]
        public string Question { get; set; }
        [Required]
        public string Answer { get; set; }
        public int Result { get; set; }

        public FeedbackInfo ToModel() => new FeedbackInfo {
            AnswerTextContent=Answer,
            FeedbackResults = Result,
            QuestionContent= Question,
            Score= Score,
            Intent= Intent,
            PutQuestionsId= UserId,
            FBId = Guid.NewGuid(),
            TimeOfOccurrence = DateTime.Now
        };
    }
}
