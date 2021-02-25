using Microsoft.Extensions.Options;
using MoMoBot.Infrastructure.Luis.Enums;
using MoMoBot.Infrastructure.Luis.Models;
using MoMoBot.Infrastructure.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MoMoBot.Infrastructure.Luis
{
    [Obsolete]
    public class LuisProgClient : ServiceClient, ILuisProgClient
    {
        private readonly LuisSettings _luisSettings;
        private readonly HttpClient _http;
        public LuisProgClient(IOptions<LuisSettings> options, HttpClient http) : base(options.Value.AuthoringKey, Location.WestUS, http)
        {
            _luisSettings = options.Value;
            _http = http;
        }

        #region 应用程序

        /// <summary>
        /// 列出所有用户应用程序
        /// </summary>
        /// <returns>List 应用程序列表</returns>
        public async Task<IReadOnlyCollection<LuisApp>> GetAllAppsAsync()
        {
            var response = await Get($"/apps");
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<IReadOnlyCollection<LuisApp>>(content);
            else if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
            {
                var exception = JsonConvert.DeserializeObject<ServiceException>(content);
                throw new Exception($"{exception.Error.Code} - {exception.Error.Message}");
            }
            return null;
        }

        /// <summary>
        /// 根据应用程序ID获取应用程序信息
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>LUIS应用程序</returns>
        public async Task<LuisApp> GetAppByIdAsync(string id)
        {
            var response = await Get($"/apps/{id}");
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<LuisApp>(content);
            else if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
            {
                var exception = JsonConvert.DeserializeObject<ServiceException>(content);
                throw new Exception($"{exception.Error.Code} - {exception.Error.Message}");
            }
            return null;
        }

        /// <summary>
        /// 根据应用程序名称获取应用程序信息
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>LUIS应用程序</returns>
        public async Task<LuisApp> GetAppByNameAsync(string name)
        {
            var apps = await GetAllAppsAsync();
            return apps.FirstOrDefault(app => app.Name.Equals(name));
        }

        /// <summary>
        /// 创建一个新的LUIS应用程序并返回id
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="description">描述</param>
        /// <param name="culture">语言</param>
        /// <param name="usageScenario">使用场景</param>
        /// <param name="domain">域</param>
        /// <param name="initialVersionId">初始版本ID</param>
        /// <returns>ID</returns>
        public async Task<string> AddAppAsync(string name, string description, string culture, string usageScenario, string domain, string initialVersionId)
        {
            var app = new
            {
                name = name,
                description = description,
                culture = culture,
                usageScenario = usageScenario,
                domain = domain,
                initialVersionId = initialVersionId
            };
            var response = await Post($"/apps", app);
            return JsonConvert.DeserializeObject<string>(response);
        }

        /// <summary>
        /// 更改LUIS应用程序的名称和描述。
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="name">新的名称</param>
        /// <param name="description">新的描述</param>
        /// <returns></returns>
        public async Task RenameAppAsync(string id, string name, string description)
        {
            var app = new
            {
                name = name,
                description = description
            };
            await Put($"/apps/{id}", app);
        }

        /// <summary>
        /// 删除应用程序
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public async Task DeleteAppAsync(string id)
        {
            await Delete($"/apps/{id}");
        }

        #endregion

        #region 意图

        /// <summary>
        /// 获取关于意图模型的信息
        /// </summary>
        /// <param name="appId">id</param>
        /// <param name="appVersionId">应用程序版本</param>
        /// <returns>应用程序的列表</returns>
        public async Task<IReadOnlyCollection<Intent>> GetAllIntentsAsync(string appId, string appVersionId)
        {
            var response = await Get($"/apps/{appId}/versions/{appVersionId}/intents");
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<IReadOnlyCollection<Intent>>(content);
            else if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
            {
                var exception = JsonConvert.DeserializeObject<ServiceException>(content);
                throw new Exception($"{ exception.Error.Code} - { exception.Error.Message}");
            }
            return null;
        }

        /// <summary>
        /// 获取关于意图模型的信息
        /// </summary>
        /// <param name="id">意图id</param>
        /// <param name="appId">id</param>
        /// <param name="appVersionId">应用程序版本</param>
        /// <returns>应用程序的意图</returns>
        public async Task<Intent> GetIntentByIdAsync(string id, string appId, string appVersionId)
        {
            var response = await Get($"/apps/{appId}/versions/{appVersionId}/intents/{id}");
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<Intent>(content);
            else if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
            {
                var exception = JsonConvert.DeserializeObject<ServiceException>(content);
                throw new Exception($"{ exception.Error.Code} - { exception.Error.Message}");
            }
            return null;
        }

        /// <summary>
        ///获取关于意图模型的信息
        /// </summary>
        /// <param name="name">意图名称</param>
        /// <param name="appId">id</param>
        /// <param name="appVersionId">应用程序版本</param>
        /// <returns>应用程序的意图</returns>
        public async Task<Intent> GetIntentByNameAsync(string name, string appId, string appVersionId)
        {
            var apps = await GetAllIntentsAsync(appId, appVersionId);
            return apps.FirstOrDefault(intent => intent.Name.Equals(name));
        }

        /// <summary>
        /// 创建一个新的app意图并返回id
        /// </summary>
        /// <param name="name">意图名称</param>
        /// <param name="appId">id</param>
        /// <param name="appVersionId">应用程序版本</param>
        /// <returns>创建意图的ID</returns>
        public async Task<string> AddIntentAsync(string name, string appId, string appVersionId)
        {
            var intent = new
            {
                name = name
            };
            var response = await Post($"/apps/{appId}/versions/{appVersionId}/intents", intent);
            return JsonConvert.DeserializeObject<string>(response);
        }

        /// <summary>
        /// 更改app意图的名称
        /// </summary>
        /// <param name="id">意图id</param>
        /// <param name="name">new 意图名称</param>
        /// <param name="appId">id</param>
        /// <param name="appVersionId">应用程序版本</param>
        /// <returns></returns>
        public async Task RenameIntentAsync(string id, string name, string appId, string appVersionId)
        {
            var intent = new
            {
                name = name
            };
            await Put($"/apps/{appId}/versions/{appVersionId}/intents/{id}", intent);
        }

        /// <summary>
        /// 从应用程序中删除意图
        /// </summary>
        /// <param name="id">意图id</param>
        /// <param name="appId">id</param>
        /// <param name="appVersionId">应用程序版本</param>
        /// <returns></returns>
        public async Task DeleteIntentAsync(string id, string appId, string appVersionId)
        {
            await Delete($"/apps/{appId}/versions/{appVersionId}/intents/{id}");
        }

        #endregion

        #region 实体

        /// <summary>
        /// 获取关于实体模型的信息
        /// </summary>
        /// <param name="appId">id</param>
        /// <param name="appVersionId">应用程序版本</param>
        /// <returns>app实体的列表</returns>
        public async Task<IReadOnlyCollection<Entity>> GetAllEntitiesAsync(string appId, string appVersionId)
        {
            var response = await Get($"/apps/{appId}/versions/{appVersionId}/entities");
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<IReadOnlyCollection<Entity>>(content);
            else if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
            {
                var exception = JsonConvert.DeserializeObject<ServiceException>(content);
                throw new Exception($"{ exception.Error.Code} - { exception.Error.Message}");
            }
            return null;
        }

        /// <summary>
        /// 获取关于实体模型的信息
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <param name="appId">id</param>
        /// <param name="appVersionId">应用程序版本</param>
        /// <returns>实体</returns>
        public async Task<Entity> GetEntityByIdAsync(string id, string appId, string appVersionId)
        {
            var response = await Get($"/apps/{appId}/versions/{appVersionId}/entities/{id}");
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<Entity>(content);
            else if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
            {
                var exception = JsonConvert.DeserializeObject<ServiceException>(content);
                throw new Exception($"{ exception.Error.Code} - { exception.Error.Message}");
            }
            return null;
        }

        /// <summary>
        /// 获取关于实体模型的信息
        /// </summary>
        /// <param name="name">实体名称</param>
        /// <param name="appId">id</param>
        /// <param name="appVersionId">应用程序版本</param>
        /// <returns>实体</returns>
        public async Task<Entity> GetEntityByNameAsync(string name, string appId, string appVersionId)
        {
            var apps = await GetAllEntitiesAsync(appId, appVersionId);
            return apps.FirstOrDefault(Entity => Entity.Name.Equals(name));
        }

        /// <summary>
        /// 创建一个新的实体并返回id
        /// </summary>
        /// <param name="name">实体名称</param>
        /// <param name="appId">id</param>
        /// <param name="appVersionId">应用程序版本</param>
        /// <returns>创建的实体的ID</returns>
        public async Task<string> AddEntityAsync(string name, string appId, string appVersionId)
        {
            var Entity = new
            {
                name = name
            };
            var response = await Post($"/apps/{appId}/versions/{appVersionId}/entities", Entity);
            return JsonConvert.DeserializeObject<string>(response);
        }

        /// <summary>
        /// 更改实体的名称
        /// </summary>
        /// <param name="id">实体id</param>
        /// <param name="name">new 意图名称</param>
        /// <param name="appId">id</param>
        /// <param name="appVersionId">应用程序版本</param>
        /// <returns></returns>
        public async Task RenameEntityAsync(string id, string name, string appId, string appVersionId)
        {
            var entity = new
            {
                name = name
            };
            await Put($"/apps/{appId}/versions/{appVersionId}/entities/{id}", entity);
        }

        /// <summary>
        /// Deletes an entity extractor from the application
        /// </summary>
        /// <param name="id">实体id</param>
        /// <param name="appId">id</param>
        /// <param name="appVersionId">应用程序版本</param>
        /// <returns></returns>
        public async Task DeleteEntityAsync(string id, string appId, string appVersionId)
        {
            await Delete($"/apps/{appId}/versions/{appVersionId}/entities/{id}");
        }

        #endregion

        #region 例子

        /// <summary>
        /// 在应用程序中添加一个标记的示例
        /// </summary>
        /// <param name="appId">id</param>
        /// <param name="appVersionId">应用程序版本</param>
        /// <param name="model">包含示例标签的对象</param>
        /// <returns>话语的对象</returns>
        public async Task<Utterance> AddExampleAsync(string appId, string appVersionId, Example model)
        {
            var response = await Post($"/apps/{appId}/versions/{appVersionId}/example", model);
            return JsonConvert.DeserializeObject<Utterance>(response);
        }

        /// <summary>
        /// 根据意图来获取意图下的所有实例话语
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appVersionId"></param>
        /// <param name="intent"></param>
        /// <returns></returns>
        public async Task<List<Examples>> GetExamplesAsync(string appId, string appVersionId, string intent)
        {
            // 把所有的短语存到缓存中
            var result = new List<Examples>();
            // 取出所有意图的短语，过滤查询的意图的短语
            // 默认分页每页500 
            var pageSize = 200;
            for (var pageIndex = 1; pageIndex < int.MaxValue; pageIndex++)
            {
                var skip = (pageIndex - 1) * pageSize;

                var response = await Get($"/apps/{appId}/versions/{appVersionId}/examples?take={pageSize}&skip={skip}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var examples = JsonConvert.DeserializeObject<List<Examples>>(json);
                    if (examples != null)
                    {
                        result.AddRange(examples.Where(e => e.intentLabel.Equals(intent))?.ToList());
                        // 当前页小于pageSize，则下一页没有数据了
                        if (examples.Count < pageSize)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// 删除实例
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appVersionId"></param>
        /// <param name="exampleId"></param>
        /// <returns></returns>

        public async Task DeleteExamplesAsync(string appId, string appVersionId, string exampleId)
        {
            await Delete($"/apps/{appId}/versions/{appVersionId}/examples/{exampleId}");
        }


        #endregion

        #region 培养

        /// <summary>
        /// 发送一个指定的LUIS应用程序版本的培训请求
        /// </summary>
        /// <param name="appId">id</param>
        /// <param name="appVersionId">应用程序版本</param>
        /// <returns>一个训练请求细节的对象</returns>
        public async Task<TrainingDetails> TrainAsync(string appId, string appVersionId)
        {
            var response = await Post($"/apps/{appId}/versions/{appVersionId}/train");
            return JsonConvert.DeserializeObject<TrainingDetails>(response);
        }

        /// <summary>
        /// 获取指定的LUIS应用程序的所有模型(意图和实体)的培训状态
        /// </summary>
        /// <param name="appId">id</param>
        /// <param name="appVersionId">应用程序版本</param>
        /// <returns>培训状态列表</returns>
        public async Task<IEnumerable<Training>> GetTrainingStatusListAsync(string appId, string appVersionId)
        {
            var response = await Get($"/apps/{appId}/versions/{appVersionId}/train");
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<IEnumerable<Training>>(content);
            else
            {
                var exception = JsonConvert.DeserializeObject<ServiceException>(content);
                throw new Exception($"{ exception.Error.Code} - { exception.Error.Message}");
            }
        }

        #endregion

        #region 发布

        /// <summary>
        /// 发布应用程序的特定版本
        /// </summary>
        /// <param name="appId">id</param>
        /// <param name="appVersionId">应用程序版本</param>
        /// <param name="isStaging"></param>
        /// <param name="region">如果应用程序是在“westeurope”中创建的，那么发布位置也是“westeurope”。对于所有其他应用程序的位置，发布位置是“westus”</param>
        /// <returns>发布细节的对象</returns>
        public async Task<Publish> PublishAsync(string appId, string appVersionId, bool isStaging, string region)
        {
            var model = new
            {
                versionId = appVersionId,
                isStaging = isStaging.ToString(),
                region = region
            };
            var response = await Post($"/apps/{appId}/publish", model);
            return JsonConvert.DeserializeObject<Publish>(response);
        }

        #endregion

    }
}
