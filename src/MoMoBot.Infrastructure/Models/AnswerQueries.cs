using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace MoMoBot.Infrastructure.Models
{
    public class AnswerQueries
    {
        public int ParameterId { get; set; }
        public Guid AnswerId { get; set; }

        [JsonIgnore]
        public QueryParameter Parameter { get; set; }
        [JsonIgnore]
        public QandA Answer { get; set; }

        /// <summary>
        /// 参数别名
        /// </summary>
        [StringLength(50)]
        public string Alias { get; set; }
    }
}
