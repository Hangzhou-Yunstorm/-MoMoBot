using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Infrastructure.Luis.Models
{
    public class Examples
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string intentLabel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> tokenizedText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<EntityLabel> entityLabels { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<IntentPredictions> intentPredictions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<EntityPredictions> entityPredictions { get; set; }
    }
    public class IntentPredictions
    {
        public string name { get; set; }
        public double score { get; set; }
    }
    public class EntityPredictions
    {
        public string entityName { get; set; }
        public int startTokenIndex { get; set; }
        public int endTokenIndex { get; set; }
        public string phrase { get; set; }
        public int entityType { get; set; }
        public string role { get; set; }
    }

}
