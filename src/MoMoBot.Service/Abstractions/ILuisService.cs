using Microsoft.Azure.CognitiveServices.Language.LUIS.Authoring.Models;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using MoMoBot.Infrastructure.Luis.Models;
using MoMoBot.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Service.Abstractions
{
    public interface ILuisService
    {
        /// <summary>
        /// 根据问题获取匹配到的意图
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        Task<LuisResult> GetIntentAsync(string question, double minimumMatchingDegree = 0.7);
        /// <summary>
        /// 查找符合的意图
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<IntentClassifier>> Search(string keyword);
        /// <summary>
        /// 获取所有的意图
        /// </summary>
        /// <param name="fromCache"></param>
        /// <returns></returns>
        Task<IList<IntentClassifier>> GetAllIntents(bool fromCache = false);
        /// <summary>
        /// 获取意图并分页
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Task<PaginationViewModel> GetIntentPaging(int limit, int offset);
        /// <summary>
        /// 获取意图下的短语实例
        /// </summary>
        /// <param name="intent"></param>
        /// <returns></returns>
        Task<IList<LabeledUtterance>> GetExamplesAsync(string intent);
        /// <summary>
        /// 重命名意图
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task RenameIntentAsync(Guid id, string name);
        /// <summary>
        /// 删除意图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteIntentAsync(Guid id);
        /// <summary>
        /// 添加意图
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<Guid> AddIntentAsync(string name);
        /// <summary>
        /// 根据id获取意图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IntentClassifier> GetIntentByIdAsync(Guid id);
        /// <summary>
        /// 添加短语实例
        /// </summary>
        /// <param name="text"></param>
        /// <param name="intentName"></param>
        /// <returns></returns>
        Task<LabelExampleResponse> AddExampleAsync(string text, string intentName);
        /// <summary>
        /// 删除短语
        /// </summary>
        /// <param name="uttId"></param>
        /// <returns></returns>
        Task DeleteUtteranceAsync(int uttId);
        /// <summary>
        /// 训练
        /// </summary>
        /// <returns></returns>
        Task<EnqueueTrainingResponse> TrainAsync();
        /// <summary>
        /// 获取训练状态
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ModelTrainingInfo>> GetTrainingStatusListAsync();
        /// <summary>
        /// 发布
        /// </summary>
        /// <returns></returns>
        Task<string> PublishAsync();

        /// <summary>
        /// 获取终结点命中历史
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        Task<EndpointHitHistory> GetEndpointHitHistory(DateTime from, DateTime to);
        Task<EndpointHitHistory> GetEndpointHitHistory(int? perDays = 7);

        /// <summary>
        /// 获取当前使用的luis app的详细信息
        /// </summary>
        /// <returns></returns>
        Task<ApplicationInfoResponse> GetCurrentLuisAppInfoAsync();

        /// <summary>
        /// 获取当前版本统计元数据
        /// </summary>
        /// <returns></returns>
        Task<LuisStatsMetadata> GetCurrentVersionStatsMetadataAsync();

        /// <summary>
        /// 获取意图分析数据
        /// </summary>
        /// <param name="intentId"></param>
        /// <returns></returns>
        Task<IntentStatsMetadata> GetIntentStatsAsync(string intentId);
    }
}
