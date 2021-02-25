using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Infrastructure.Luis.Models
{
    public class IntentPredictionsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 出差
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double Score { get; set; }
    }

    public class EntityLabelItem
    {
        public string Id { get; set; }
        public string EntityName { get; set; }
        public int StartTokenIndex { get; set; }
        public int EndTokenIndex { get; set; }
        public object Role { get; set; }
        public int EntityType { get; set; }
        public string RoleId { get; set; }
    }

    public class EntityPredictionItem
    {
        public string Id { get; set; }
        public string EntityName { get; set; }
        public int StartTokenIndex { get; set; }
        public int EndTokenIndex { get; set; }
        public string Phrase { get; set; }
        public int EntityType { get; set; }
        public string Role { get; set; }
    }

    public class UtterancesItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 因公出差
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> TokenizedText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IntentId { get; set; }
        /// <summary>
        /// 出差
        /// </summary>
        public string IntentLabel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<EntityLabelItem> EntityLabels { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<IntentPredictionsItem> IntentPredictions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<EntityPredictionItem> EntityPredictions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TokenMetadata { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PatternPredictions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SentimentAnalysis { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double LabeledIntentScore { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreatedDateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ConnectedServiceResult { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<EntityPredictionItem> MisclassifiedIntentPredictions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ModifiedDateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MultiIntentPredictions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<IntentPredictionsItem> AmbiguousIntentPredictions { get; set; }
    }

    public class IntentStatsMetadata
    {
        /// <summary>
        /// 
        /// </summary>
        public string SchemaVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AppVersionMetadata { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<UtterancesItem> Utterances { get; set; }
    }
}
