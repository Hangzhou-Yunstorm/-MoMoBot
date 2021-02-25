using EasyCaching.Core;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoMoBot.Api.ViewModels;
using MoMoBot.Infrastructure;
using MoMoBot.Infrastructure.Cache;
using MoMoBot.Infrastructure.DTalk;
using MoMoBot.Infrastructure.Models;
using MoMoBot.Infrastructure.ViewModels;
using MoMoBot.Service.Abstractions;
using MoMoBot.Service.Implements;
using MoMoBot.Service.Map;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MoMoBot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        #region Init
        private readonly KnowledgeMapContext _mapContext;
        private readonly ILogger<MessageController> _logger;
        private readonly ILuisService _luis;
        private readonly IAnswerService _answerService;
        private readonly IUnknownService _issueService;
        private readonly IRedisCachingProvider _redis;
        private readonly IDepartmentService _departService;
        private readonly IPermissionService _permissionService;
        private readonly IFeedbackService _feedbackService;
        private readonly DingDingApprovalService _ddApprovalService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly DingTalkHelper _ddHelper;
        private readonly IAppSettings _settings;
        public MessageController(ILuisService luis,
            IAnswerService answerService,
            IUnknownService issueService,
            IRedisCachingProvider redis,
            DingTalkHelper ddHelper,
            IDepartmentService departmentService,
            IPermissionService permissionService,
            IConfiguration configuration,
            IFeedbackService feedbackService,
            IAppSettings settings,
            ILogger<MessageController> logger,
            KnowledgeMapContext mapContext,
            DingDingApprovalService dingDingApprovalService)
        {
            _logger = logger;
            _settings = settings;
            _ddHelper = ddHelper;
            _configuration = configuration;
            _ddApprovalService = dingDingApprovalService;
            _luis = luis;
            _answerService = answerService;
            _issueService = issueService;
            _departService = departmentService;
            _redis = redis;
            _feedbackService = feedbackService;
            _permissionService = permissionService;
            _httpClient = HttpClientFactory.Create();

            _mapContext = mapContext;
        }
        #endregion

        #region Action Methods
        [HttpPost()]
        public async Task<IActionResult> Post(string question,
            [FromHeader(Name = "x-dd-userid")]string userId,
            [FromHeader(Name = "x-yc-conversationid")]string conversationId)
        {
            // continue
            if (string.IsNullOrEmpty(conversationId) == false)
            {
                return await ContinueConversation(question, conversationId, userId);
            }

            var minimumMatchingDegree = await _settings.GetIntentMinimumMatchingDegreeAsync(0.7d);
            _logger.LogInformation($"最小匹配度为：【{minimumMatchingDegree}】");
            if (string.IsNullOrWhiteSpace(question))
            {
                return Ok(new MessageResponse(true, "不好意思，没有听明白你的意思。"));
                //return Ok(new { success = true, message = $"不好意思，没有听明白你的意思。" });
            }
            var intent = await _luis.GetIntentAsync(question, minimumMatchingDegree);
            if (intent != null)
            {
                var matchIntent = intent.TopScoringIntent;
                if (matchIntent != null)
                {
                    // 当前钉钉用户
                    var ddUser = await GetCurrentDDUserInfoAsync(userId);
                    _answerService.SetCurrentDingDingUser(ddUser);
                    // 获取当前钉钉用户所在部门及父级部门
                    var departIds = GetCurrentDDUserDepartIds(ddUser);

                    await RecordMatchIntentCountAsync(matchIntent.Intent);

                    // 获取答案（权限过滤）
                    var answer = await _answerService.GetAnswerByIntent(intent.TopScoringIntent.Intent, departIds);

                    return Ok(new MessageResponse(true, answer.Answer, answer.AnswerType == Infrastructure.Enums.AnswerTypes.ProcessFlow, new { answer.ConversationId, intent.TopScoringIntent.Score, intent.TopScoringIntent.Intent, question, userId }));
                    //return Ok(new { success = true, message = answer.Answer, data = new { answer.ConversationId, intent.TopScoringIntent.Score, intent.TopScoringIntent.Intent, question, userId } });
                }
            }
            // 未找到意图，记录问题
            await _issueService.Record(new Unknown { Id = Guid.NewGuid(), Content = question, TimeOfOccurrence = DateTime.Now, Remarks = "此问题未匹配到意图", Type = 1 });
            return Ok(new MessageResponse(true, "不好意思，没有听明白你的意思。"));
            //return Ok(new { success = true, message = $"不好意思，没有听明白你的意思。" });
        }

        //[HttpPost("continue")]
        //public async Task<IActionResult> Continue(string answer,
        //    [FromHeader(Name = "x-yc-conversationid")]string conversationId,
        //    [FromHeader(Name = "x-dd-userid")]string userId)
        //{
        //    await _mapContext.LoadFromChacheAsync(conversationId);
        //    if (_mapContext.HasNext())
        //    {
        //        var step = await _mapContext.ContinueAsync(answer, conversationId);
        //        if (step != null)
        //        {
        //            return Ok(new { success = true, message = step.Question, data = new { end = false, conversationId, userId } });
        //        }
        //    }

        //    var result = await _mapContext.GetStateAsync(conversationId);
        //    await _mapContext.DestroyAsync(conversationId);
        //    return Ok(new { success = true, message = JsonConvert.SerializeObject(result), data = new { end = true, conversationId, userId } });
        //}

        [HttpPost("multiple")]
        public async Task<IActionResult> MultipleRoundsAsync(string title,
            [FromHeader(Name = "x-dd-userid")]string userId)
        {
            if (string.IsNullOrEmpty(title))
            {
                return Ok();
            }
            // 当前钉钉用户
            var ddUser = await GetCurrentDDUserInfoAsync(userId);
            // 获取当前钉钉用户所在部门及父级部门
            var departIds = GetCurrentDDUserDepartIds(ddUser);

            var parameters = new Dictionary<string, string>();
            parameters.Add("title", DingDingScrypt.jEncrypt(title));
            var url = await _settings.GetAsync(SettingKeys.BusinessInquiryUrl) ??
                throw new ArgumentNullException(SettingKeys.BusinessInquiryUrl);
            var result = await PostRequest<List<ModularResponseResult>>($"{url}/api/Employee/GetSearchRestultByTitle", parameters);

            // None
            if (result?.Count <= 0)
            {
                // 未找到意图，记录问题
                await _issueService.Record(new Unknown { Id = Guid.NewGuid(), Content = title, TimeOfOccurrence = DateTime.Now, Remarks = "此问题未匹配到意图", Type = 1 });
            }
            // 过滤权限
            var finalResult = await _permissionService.FilterModulePermissionAsync(departIds, result);

            return Ok(finalResult);
        }

        [HttpPost("final")]
        public async Task<IActionResult> Final(string listName, string id, string colummName = "")
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("listName", DingDingScrypt.jEncrypt(listName));
            parameters.Add("id", DingDingScrypt.jEncrypt(id));
            parameters.Add("colummName", DingDingScrypt.jEncrypt(colummName ?? ""));

            var url = await _settings.GetAsync(SettingKeys.BusinessInquiryUrl) ??
                throw new ArgumentNullException(SettingKeys.BusinessInquiryUrl);
            var result = await PostRequest<List<ModularResponseFinnalResult>>($"{url}/api/Employee/GetSearchRestultByColumn", parameters);
            return Ok(result?.Where(r => !string.IsNullOrEmpty(r.TValue)));
        }

        [HttpPost("feedback")]
        public async Task<IActionResult> Feedback([FromBody]InputFeedbackViewModel vm)
        {
            await _feedbackService.AddAsync(vm.ToModel());
            return Ok();
        }

        [HttpPost("travelapproval")]
        public async Task<IActionResult> TravelApproval([FromBody]InputTravelApprovalViewModel vm)
        {
            if (vm.EndTime <= vm.StartTime)
            {
                return BadRequest();
            }
            if (!string.IsNullOrEmpty(vm.UserId) && !string.IsNullOrEmpty(vm.DepartId))
            {
                var approvers = _configuration.GetValue("Approvers", "");
                if (!string.IsNullOrEmpty(approvers))
                {
                    await _ddApprovalService.LaunchTravelApprovalAsync(vm.ConvertToRequestModel(approvers));
                }
            }

            return Ok();
        }
        #endregion

        #region Private Methods
        private List<long> GetCurrentDDUserDepartIds(DingDingUser ddUser, long? parentId = null)
        {
            var result = new List<long>();
            if (ddUser != null)
            {
                var departments =  // parentId.HasValue ? _departService.Find(d => d.Id == parentId.Value) :
                    _departService.Find(o => ddUser.Departments.Any(d => d == o.DepartName));
                if (departments != null)
                {
                    result.AddRange(departments.Select(o => o.Id).ToList());
                    //foreach (var department in departments)
                    //{
                    //    if (department.ParentId != null)
                    //    {
                    //        result.AddRange(GetCurrentDDUserDepartIds(ddUser, department.ParentId));
                    //    }
                    //}
                }
            }
            return result;
        }

        private async Task RecordMatchIntentCountAsync(string intent)
        {
            if (intent != "None")
            {
                await _redis.RPushAsync(Constants.RedisKey.MatchIntents, new List<string> { intent });
            }
        }

        private async Task<DingDingUser> GetCurrentDDUserInfoAsync(string userId)
        {
            DingDingUser user = null;
            if (!string.IsNullOrEmpty(userId))
            {
                var key = $"dduser_{userId}";
                var keyValues = await _redis.HMGetAsync(key, DingDingUser.GetFields());
                user = keyValues.GetUserInfoFromDictionary();
                if (string.IsNullOrWhiteSpace(user?.UserId) || string.IsNullOrWhiteSpace(user?.UserName))
                {
                    user = await _ddHelper.GetCurrentDDUserInfoAsync(userId);
                    if (user != null)
                    {
                        await _redis.HMSetAsync(key, user.ToKeyValues());
                    }
                }
            }
            return user;
        }

        private async Task<T> PostRequest<T>(string url, Dictionary<string, string> parameters)
            where T : class
        {
            try
            {
                if (parameters.ContainsKey("token") == false)
                {
                    parameters.Add("token", DingDingScrypt.jEncrypt("DING" + DingDingScrypt.GetTimeSpanByDate()));
                }
                var response = await _httpClient.PostAsync($"{url}?{GetUrlParameter(parameters)}", null);
                if (response.IsSuccessStatusCode)
                {
                    var str = (await response.Content.ReadAsStringAsync()).Replace("\\", "");
                    if (str.Length >= 2)
                    {
                        var json = str.Remove(0, 1).Remove(str.Length - 2, 1);
                        return JsonConvert.DeserializeObject<T>(json);
                    }
                }
            }
            catch (Exception)
            {
            }
            return default(T);
        }

        private string GetUrlParameter(Dictionary<string, string> parameters)
        {
            var result = new StringBuilder();

            foreach (var key in parameters?.Keys)
            {
                result.Append($"{key}={parameters[key]}&");
            }
            return result.ToString();
        }

        private async Task<IActionResult> ContinueConversation(string answer, string conversationId, string userId)
        {
            await _mapContext.LoadFromChacheAsync(conversationId);
            if (_mapContext.HasNext())
            {
                var questions = new List<string>();
                ContinueResult result;
                do
                {
                    result = await _mapContext.ContinueAsync(answer, conversationId);
                    if (result != null)
                    {
                        questions.Add(result.Question);
                    }
                } while (result?.IsQuestion == false);

                return Ok(new MessageResponse(true, string.Join('，', questions), true, new { end = false, conversationId, userId }));
                //return Ok(new { success = true, message = result.Question, data = new { result.IsQuestion, end = false, conversationId, userId } });

            }

            var values = await _mapContext.GetStateAsync(conversationId);
            await _mapContext.DestroyAsync(conversationId);
            return Ok(new MessageResponse(true, "ok", true, new { end = true, conversationId, userId, values }));
            //return Ok(new { success = true, message = JsonConvert.SerializeObject(values), data =  });

        }
        #endregion
    }
}
