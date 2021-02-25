using System;
using System.Collections.Generic;
using System.Text;

namespace MoMoBot.Infrastructure.Luis.Models
{
    public class ModelsMetadataItem
    {
        /// <summary>
        /// id
        /// </summary>
        public string ModelId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string ModelName { get; set; }
        /// <summary>
        /// 短语实例数量
        /// </summary>
        public int UtterancesCount { get; set; }
        /// <summary>
        /// 错误分类短语数量
        /// </summary>
        public int MisclassifiedUtterancesCount { get; set; }
        /// <summary>
        /// 含糊不清短语数量
        /// </summary>
        public int AmbiguousUtterancesCount { get; set; }
        /// <summary>
        /// 错误分类含糊不清短语数量
        /// </summary>
        public int MisclassifiedAmbiguousUtterancesCount { get; set; }
    }

    public class LuisStatsMetadata
    {
        /// <summary>
        /// 短语数量
        /// </summary>
        public int AppVersionUtterancesCount { get; set; }
        /// <summary>
        /// 模型元数据
        /// </summary>
        public List<ModelsMetadataItem> ModelsMetadata { get; set; }
        /// <summary>
        /// 意图数量
        /// </summary>
        public int IntentsCount { get; set; }
        /// <summary>
        /// 实体数量
        /// </summary>
        public int EntitiesCount { get; set; }
        /// <summary>
        /// 短语列表数量
        /// </summary>
        public int PhraseListsCount { get; set; }
        /// <summary>
        /// 模式数量
        /// </summary>
        public int PatternsCount { get; set; }
        /// <summary>
        /// 训练时间
        /// </summary>
        public string TrainingTime { get; set; }
        /// <summary>
        /// 最后训练时间
        /// </summary>
        public string lastTrainDate { get; set; }
        /// <summary>
        /// 错误分类短语数量
        /// </summary>
        public int MisclassifiedUtterancesCount { get; set; }
        /// <summary>
        /// 含糊不清短语数量
        /// </summary>
        public int AmbiguousUtterancesCount { get; set; }
        /// <summary>
        /// 错误分类含糊不清短语数量
        /// </summary>
        public int MisclassifiedAmbiguousUtterancesCount { get; set; }
    }
}
