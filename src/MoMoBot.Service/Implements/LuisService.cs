using Microsoft.Azure.CognitiveServices.Language.LUIS.Authoring;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Authoring.Models;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoMoBot.Infrastructure.Cache;
using MoMoBot.Infrastructure.Luis.Models;
using MoMoBot.Infrastructure.Models;
using MoMoBot.Infrastructure.ViewModels;
using MoMoBot.Service.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MoMoBot.Service.Implements
{
    /// <summary>
    /// luis api调用
    /// </summary>
    public class LuisService
        : ILuisService
    {
        private readonly ILUISAuthoringClient _luisAuthoringClient;
        private readonly ILUISRuntimeClient _luisRuntimeClient;
        private readonly LuisSettings _luisSettings;
        private readonly ILogger<LuisService> _logger;
        private readonly IRedisCacheService _redisCache;
        private readonly HttpClient _httpClient;
        public LuisService(IOptions<LuisSettings> options,
            ILUISAuthoringClient luisAuthoringClient,
            ILUISRuntimeClient luisRuntimeClient,
            IRedisCacheService redisCache,
            HttpClient httpClient,
            ILogger<LuisService> logger)
        {
            _redisCache = redisCache;
            _luisSettings = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
            _httpClient = httpClient;

            _luisRuntimeClient = luisRuntimeClient;
            _luisAuthoringClient = luisAuthoringClient;
        }

        /// <summary>
        /// 根据提问获取其中的意图
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public async Task<LuisResult> GetIntentAsync(string question, double minimumMatchingDegree = 0.7)
        {
            var result = await _luisRuntimeClient.Prediction.ResolveAsync(_luisSettings.AppId.ToString(), question);
            if (result != null)
            {
                // 匹配度小于最小匹配度
                if (result.TopScoringIntent?.Score < minimumMatchingDegree)
                {
                    return new LuisResult
                    {
                        TopScoringIntent = new IntentModel
                        {
                            Score = 1,
                            Intent = "None"
                        }
                    };
                }
            }

            return result;

            //var queryString = HttpUtility.ParseQueryString(string.Empty);

            //// This app ID is for a public sample app that recognizes requests to turn on and turn off lights
            //var luisAppId = _luisSettings.AppId;

            //// The request header contains your subscription key
            //_httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _luisSettings.SubscriptionKey);

            //// The "q" parameter contains the utterance to send to LUIS
            //queryString["q"] = question;

            //// These optional request parameters are set to their default values
            //queryString["timezoneOffset"] = "0";
            //queryString["verbose"] = "false";
            //queryString["spellCheck"] = "false";
            //queryString["staging"] = "false";

            //var uri = $"https://{_luisSettings.Location}.api.cognitive.microsoft.com/luis/v2.0/apps/{luisAppId}?{queryString}";
            //var response = await _httpClient.GetAsync(uri);
            //if (response.IsSuccessStatusCode)
            //{
            //    var strResponseContent = await response.Content.ReadAsStringAsync();
            //    var intent = JsonConvert.DeserializeObject<IntentResult>(strResponseContent);
            //    if (intent != null)
            //    {
            //        // 匹配度小于最小匹配度
            //        if (intent.topScoringIntent?.score < minimumMatchingDegree)
            //        {
            //            return new IntentResult
            //            {
            //                topScoringIntent = new TopScoringIntent
            //                {
            //                    score = 1,
            //                    intent = "None"
            //                }
            //            };
            //        }
            //    }

            //    return intent;
            //}
            //_logger.LogError($"获取意图失败：{response}");
            //return null;
        }

        public async Task<PaginationViewModel> GetIntentPaging(int limit, int offset)
        {
            var result = new PaginationViewModel();
            var all = await _luisAuthoringClient.Model.ListIntentsAsync(_luisSettings.AppId, _luisSettings.LuisVersion, 0, 500);
            if (all != null)
            {
                result.Rows = all.Skip(offset)
                    .Take(limit);

                result.Total = all.Count;
            }
            return result;
        }

        public async Task<IList<LabeledUtterance>> GetExamplesAsync(string intent)
        {
            var examples = await _luisAuthoringClient.Examples.ListAsync(_luisSettings.AppId, _luisSettings.LuisVersion, take: 500);
            return examples.Where(e => e.IntentLabel == intent).ToList();
        }

        public async Task<List<IntentClassifier>> Search(string keyword)
        {
            var intents = await _luisAuthoringClient.Model.ListIntentsAsync(_luisSettings.AppId, _luisSettings.LuisVersion, 0, 500);
            if (!string.IsNullOrEmpty(keyword))
            {
                return intents.Where(i => i.Name.Contains(keyword))?.ToList();
            }
            return intents?.ToList();
        }

        public async Task RenameIntentAsync(Guid id, string name)
        {
            var update = new ModelUpdateObject(name);
            await _luisAuthoringClient.Model.UpdateIntentAsync(_luisSettings.AppId, _luisSettings.LuisVersion, id, update);
        }

        public async Task DeleteIntentAsync(Guid id)
        {
            await _luisAuthoringClient.Model.DeleteIntentAsync(_luisSettings.AppId, _luisSettings.LuisVersion, id);
        }

        public async Task<Guid> AddIntentAsync(string name)
        {
            var create = new ModelCreateObject(name);
            return await _luisAuthoringClient.Model.AddIntentAsync(_luisSettings.AppId, _luisSettings.LuisVersion, create);
        }

        public async Task<IList<IntentClassifier>> GetAllIntents(bool fromCache = false)
        {
            IList<IntentClassifier> intents = null;
            if (fromCache)
            {
                intents = await _redisCache.StringGetAsync<IList<IntentClassifier>>("aichat-intents");
            }

            if (intents == null)
            {
                intents = await _luisAuthoringClient.Model.ListIntentsAsync(_luisSettings.AppId, _luisSettings.LuisVersion);
                if (intents != null)
                {
                    await _redisCache.StringSetAsync("aichat-intents", intents);
                }
            }
            return intents?.ToList();
        }

        public async Task<EnqueueTrainingResponse> TrainAsync()
        {
            var details = await _luisAuthoringClient.Train.TrainVersionAsync(_luisSettings.AppId, _luisSettings.LuisVersion);
            return details;
        }

        public async Task<IEnumerable<ModelTrainingInfo>> GetTrainingStatusListAsync()
        {
            var status = await _luisAuthoringClient.Train.GetStatusAsync(_luisSettings.AppId, _luisSettings.LuisVersion);
            return status;
        }

        public async Task<string> PublishAsync()
        {
            try
            {
                var publishObj = new ApplicationPublishObject(versionId: _luisSettings.LuisVersion, isStaging: false);
                var info = await _luisAuthoringClient.Apps.PublishAsync(_luisSettings.AppId, publishObj);
                // var publish = await _luisClient.PublishAsync(_luisSettings.AppId, _luisSettings.LuisVersion, false, "southeastasia,westus");
                return "发布成功";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<IntentClassifier> GetIntentByIdAsync(Guid id)
        {
            return await _luisAuthoringClient.Model.GetIntentAsync(_luisSettings.AppId, _luisSettings.LuisVersion, id);
            // return await _luisClient.GetIntentByIdAsync(id.ToString(), _luisSettings.AppId, _luisSettings.LuisVersion);
        }

        public async Task<LabelExampleResponse> AddExampleAsync(string text, string intentName)
        {
            var example = new Example()
            {
                Text = text,
                IntentName = intentName
            };
            var createObj = new ExampleLabelObject(text, intentName: intentName);
            return await _luisAuthoringClient.Examples.AddAsync(_luisSettings.AppId, _luisSettings.LuisVersion, createObj);
            // return await _luisClient.AddExampleAsync(_luisSettings.AppId, _luisSettings.LuisVersion, example);
        }

        public async Task DeleteUtteranceAsync(int uttId)
        {
            await _luisAuthoringClient.Examples.DeleteAsync(_luisSettings.AppId, _luisSettings.LuisVersion, uttId);
            // await _luisClient.DeleteExamplesAsync(_luisSettings.AppId, _luisSettings.LuisVersion, uttId);
        }

        public async Task<EndpointHitHistory> GetEndpointHitHistory(int? perDays = 7)
        {
            var now = DateTime.Now;
            var from = now.AddDays(1 - perDays.Value);
            var to = now;
            return await GetEndpointHitHistory(from, to);
        }
        public async Task<EndpointHitHistory> GetEndpointHitHistory(DateTime from, DateTime to)
        {
            var url = $"https://westus.api.cognitive.microsoft.com/luis/webapi/v2.0/apps/{_luisSettings.AppId}/versions/{_luisSettings.LuisVersion}/stats/endpointhitshistory?from={from.ToString()}&to={to.ToString()}";
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _luisSettings.AuthoringKey);
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<EndpointHitHistory>(body);
            }
            return null;
        }

        public async Task<ApplicationInfoResponse> GetCurrentLuisAppInfoAsync()
        {
            var appid = _luisSettings.AppId;
            return await _luisAuthoringClient.Apps.GetAsync(appid);
        }

        public async Task<LuisStatsMetadata> GetCurrentVersionStatsMetadataAsync()
        {
            var url = $"{_luisSettings.AuthoringEndpoint}/luis/webapi/v2.0/apps/{_luisSettings.AppId}/versions/{_luisSettings.LuisVersion}/statsmetadata";
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _luisSettings.AuthoringKey);
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<LuisStatsMetadata>(json);
            }
            return null;
        }

        public async Task<IntentStatsMetadata> GetIntentStatsAsync(string intentId)
        {
            if (!string.IsNullOrEmpty(intentId))
            {
                var url = $"{_luisSettings.AuthoringEndpoint}/luis/webapi/v2.0/apps/{_luisSettings.AppId}/versions/{_luisSettings.LuisVersion}/intents/{intentId}/stats";
                _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _luisSettings.AuthoringKey);
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IntentStatsMetadata>(json);
                }
            }
            return null;
        }
    }
}
