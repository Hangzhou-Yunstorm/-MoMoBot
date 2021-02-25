using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.ViewModels
{
    public class UpdateKnowledgeViewModel
    {
        public Guid Id { get; set; }
        public string Answer { get; set; }
        public string RequestUrl { get;  set; }
        public int Method { get;  set; }
        public List<int> ParameterIds { get; set; }
        public int AnswerType { get; set; }
        public string FlowId { get; set; }
        [Required]
        public string Intent { get; set; }
    }
}
