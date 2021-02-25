using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions;
using Microsoft.Extensions.Logging;
using MoMoBot.Core;
using MoMoBot.Infrastructure.Cache;
using MoMoBot.Infrastructure.Database;
using MoMoBot.Infrastructure.Enums;
using MoMoBot.Infrastructure.Models;
using MoMoBot.Infrastructure.Models.KnowledgeMapModel;
using MoMoBot.Service.Abstractions;
using MoMoBot.Service.Dtos;
using MoMoBot.Service.Map;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
namespace MoMoBot.Service.Implements
{
    public class AnswerService : IAnswerService
    {
        private readonly MoMoDbContext _context;
        private readonly IRedisCacheService _cache;
        private readonly IUnknownService _issueService;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AnswerService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly KnowledgeMapContext _mapContext;
        private DingDingUser _ddUser;
        public AnswerService(MoMoDbContext context,
            IRedisCacheService cache,
            IHttpContextAccessor httpContextAccessor,
            IUnknownService issueService,
            HttpClient httpClient,
            KnowledgeMapContext mapContext,
            ILogger<AnswerService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _issueService = issueService;
            _cache = cache;
            _context = context;
            _httpClient = httpClient;
            _logger = logger;
            _mapContext = mapContext;
        }

        public async Task<QandA> AddAsync(QandA qandA)
        {
            var entry = await _context.Answers.AddAsync(qandA);
            return entry.Entity;
        }

        public async Task AddRangeAsync(List<QandA> qandAs)
        {
            if (qandAs != null)
            {
                foreach (var item in qandAs)
                {
                    await AddAsync(item);
                }
            }
        }

        public async Task<List<QandA>> GetAllQandAsAsync()
        {
            return await _context.Answers?.OrderBy(a => a.Intent).ToListAsync();
        }

        public async Task<DialogDto> GetAnswerByIntent(string intent, List<long> departIds, string conversationId = null)
        {
            if (string.IsNullOrEmpty(conversationId))
            {
                conversationId = Guid.NewGuid().ToString();
            }

            if (!string.IsNullOrEmpty(intent))
            {
                // 获取回答列表
                var answers = await GetAnswers(intent, departIds);
                QandA answer = null;
                if (answers?.Count > 0)
                {
                    // 随机一个回复
                    if (answers.Count > 1)
                    {
                        var r = new Random();
                        var index = r.Next(0, answers.Count);
                        answer = answers[index];
                    }
                    else
                    {
                        answer = answers.First();
                    }
                }
                if (answer != null)
                {
                    switch (answer.AnswerType)
                    {
                        case AnswerTypes.ProcessFlow:
                            return await StartFlowAnswer(answer, conversationId);
                        default:
                            return await ReplaceAnswer(answer, string.Empty);
                    }
                }
                // 未找到答案，记录此意图
                await _issueService.Record(new Unknown { Id = Guid.NewGuid(), Content = intent, TimeOfOccurrence = DateTime.Now, Remarks = "此意图未设置回复内容", Type = 2 });
            }
            return new DialogDto(string.Empty, "不好意思，我不明白你的意思！");
        }

        public async Task<List<QandA>> GetPagingQandAsAsync(int skipNum, int count)
        {
            return await _context.Answers
                .OrderBy(a => a.Answer)
                .OrderBy(a => a.Intent)
                .Skip(skipNum)
                .Take(count)
                .ToListAsync();
        }

        public async Task<bool> RemoveById(Guid Id)
        {
            QandA answer = await _context.Answers.FirstOrDefaultAsync(i => i.Id == Id);
            _context.Entry(answer).State = EntityState.Deleted;
            return true;
        }

        public async Task<QandA> GetAsync(Guid id)
        {
            return await _context.Answers.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task UpdateAsync(QandA qandA)
        {
            await Task.Run(() =>
            {
                var entry = _context.Entry(qandA);
                entry.State = EntityState.Unchanged;
                entry.Property(a => a.Answer).IsModified = true;
                entry.Property(a => a.RequestUrl).IsModified = true;
                entry.Property(a => a.Method).IsModified = true;
            });
        }

        public async Task UpdateAllProperties(QandA qandA)
        {
            await Task.Run(() =>
            {
                var entry = _context.Entry(qandA);
                entry.State = EntityState.Modified;
            });
        }

        public async Task<string> GetJsonDataAsync()
        {
            // 获取所有数据
            var data = await GetAllQandAsAsync();
            if (data != null)
            {
                var list = data.Select(q => new { q.Answer, q.Intent });
                var json = JsonConvert.SerializeObject(list);
                return json;
            }
            return string.Empty;
        }

        public async Task<bool> Exist(string intent, string answer)
        {
            return await Task.FromResult(_context.Answers.Any(a => a.Answer.Equals(answer) &&
           a.Intent.Equals(intent)));
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Answers.CountAsync();
        }

        public async Task AddParameters(List<AnswerQueries> answerQueries)
        {
            if (answerQueries != null)
            {
                await _context.AnswerQueries.AddRangeAsync(answerQueries);
            }
        }

        public async Task RemoveParameters(Guid answerId)
        {
            await Task.Run(() =>
            {
                var answerQueries = _context.AnswerQueries
                .Where(q => q.AnswerId == answerId);
                if (answerQueries != null)
                {
                    _context.AnswerQueries.RemoveRange(answerQueries);
                }
            });
        }

        /// <summary>
        /// 获取配置的参数
        /// </summary>
        /// <returns></returns>
        public async Task<QandA> GetParameterSettings(Guid answerId)
        {
            var settings = await _context.Answers
                .Include(a => a.AnswerQueries)
                .FirstOrDefaultAsync(a => a.Id == answerId);
            return settings;
        }

        public async Task ClearCache()
        {
            var keys = await _context.Answers
                .Select(a => a.Intent)
                .Distinct()
                ?.ToListAsync();
            await _cache.KeyListDeleteAsync(keys);
        }

        private async Task<List<QandA>> GetAnswers(string intent, List<long> departIds)
        {
            var sql = new StringBuilder();
            sql.Append("SELECT a.* FROM \"Answers\" AS a WHERE a.\"Intent\"=@intent");
            if (departIds?.Count > 0)
            {
                var ids = string.Join(",", departIds);
                sql.Append($" AND(a.\"Id\" IN(SELECT DISTINCT b.\"QAId\" FROM \"QAPermissions\" AS b WHERE b.\"DepartmentId\" IN({ids})) OR a.\"IsPublic\" = TRUE )");
            }
            else
            {
                sql.Append(" AND a.\"IsPublic\" = TRUE");
            }

            // SELECT a.* FROM "Answers" AS a WHERE a."Intent"='问候' AND(a."Id" IN(SELECT DISTINCT b."QAId" FROM "QAPermissions" AS b WHERE b."DepartmentId" IN(1, 2, 3)) OR a."IsPublic" = TRUE )
            var answers = await _context.SqlQueryAsync<QandA>(sql.ToString(), new { intent }); //await _cache.StringGetAsync<List<QandA>>(intent);
                                                                                               //if (answers == null)
                                                                                               //{
                                                                                               //    answers = await _context.Answers
                                                                                               //        .Where(a => a.Intent.Equals(intent, StringComparison.CurrentCultureIgnoreCase))?
                                                                                               //        .ToListAsync();
                                                                                               //    if (answers != null)
                                                                                               //    {
                                                                                               //        await _cache.StringSetAsync(intent, answers, TimeSpan.FromMinutes(20));
                                                                                               //    }
                                                                                               //}
            return answers?.ToList();
        }


        public async Task<List<QandA>> SearchAsync(string Keyword, int limit, int offset)
        {
            if (string.IsNullOrEmpty(Keyword) == false)
            {
                return await _context.Answers
                    .Where(a => a.Answer.Contains(Keyword) || a.Intent.Contains(Keyword))
                    .OrderBy(a => a.Intent)
                    .Skip(offset)
                    .Take(limit)?
                    .ToListAsync();
            }
            else
            {
                return await _context.Answers
                    .OrderBy(a => a.Intent)
                    .Skip(offset)
                    .Take(limit)?
                    .ToListAsync();
            }

        }

        public async Task<int> GetTotalCountAsync(string Keyword)
        {
            if (string.IsNullOrEmpty(Keyword) == false)
            {
                return await _context.Answers.Where(a => a.Answer.Contains(Keyword) || a.Intent.Contains(Keyword)).CountAsync();
            }
            else
            {
                return await _context.Answers.CountAsync();
            }
        }

        public void SetCurrentDingDingUser(DingDingUser user)
        {
            _ddUser = user;
        }


        /// <summary>
        /// 请求api替换其中占位符
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="dingCode"></param>
        /// <returns></returns>
        private async Task<DialogDto> ReplaceAnswer(QandA answer, string conversationId)
        {
            if (!string.IsNullOrWhiteSpace(answer.RequestUrl))
            {
                if (string.IsNullOrWhiteSpace(_ddUser?.UserId))
                {
                    // 没有获取到钉钉userId
                    _logger.LogError("没有获取到 当前钉钉用户信息。");
                    return new DialogDto(conversationId, "不好意思，小摩摩未能认出你，所以不能回答你😥。");
                }

                var parameters = new Dictionary<string, string>();
                var queryParams = await _context.AnswerQueries
                    .Where(q => q.AnswerId == answer.Id)
                    .Include(q => q.Parameter)?
                    .ToListAsync();
                if (queryParams?.Count > 0)
                {
                    foreach (var item in queryParams)
                    {
                        var key = string.IsNullOrEmpty(item.Alias) ? item.Parameter.Name : item.Alias;
                        var value = GetParameterValue(item.Parameter.Name);
                        parameters.Add(key, value);
                    }
                }
                parameters.Add("token", DingDingScrypt.jEncrypt("DING" + DingDingScrypt.GetTimeSpanByDate()));
                var holder = await SendHttpRequestAsync((RequestMethods)answer.Method, answer.RequestUrl, parameters);
                // 发生了错误
                if (holder.Contains("ERROR"))
                {
                    _logger.LogError($"请求值发生错误 {holder}");
                    answer.Answer = "没有找到相关信息！";
                }
                else
                {
                    answer.Answer = answer.Answer.Replace("[$h]", holder);
                }
            }
            return new DialogDto(conversationId, answer.Id, answer.Intent, answer.Answer);
        }

        /// <summary>
        /// 获取请求参数的值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dingUserId">钉钉用户id</param>
        /// <returns></returns>
        private string GetParameterValue(string name)
        {
            // TODO : 参数获取途径
            if (name.Equals("userName", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(_ddUser.UserName))
                {
                    return DingDingScrypt.jEncrypt(_ddUser.UserName);
                }
            }
            else if (name.Equals("email", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(_ddUser?.Email))
                {
                    return DingDingScrypt.jEncrypt(_ddUser.Email);
                }
            }
            else
            {
                _logger.LogError($"请求参数{name}没有设定值！");
            }
            return string.Empty;
        }

        private async Task<string> SendHttpRequestAsync(RequestMethods methods, string url, Dictionary<string, string> parameters)
        {
            if (methods == RequestMethods.Get)
            {
                return await _httpClient.GetStringAsync($"{url}?{GetUrlParameter(parameters)}");
            }
            else if (methods == RequestMethods.Post)
            {
                // StringContent，FormUrlEncodedContent，MultipartFormDataContent 
                //var content = new FormUrlEncodedContent(parameters);
                var response = await _httpClient.PostAsync($"{url}?{GetUrlParameter(parameters)}", null);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                _logger.LogError($"请求[{response.RequestMessage.Method.Method}] {response.RequestMessage.RequestUri} 返回{response.StatusCode} : {await response.Content.ReadAsStringAsync()}");
            }
            return string.Empty;
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

        private async Task<DialogDto> StartFlowAnswer(QandA answer, string conversationId)
        {
            MoMoBotAssert.ValueNullOrWhiteSpace(answer.FlowId);

            var start = await _mapContext.StartAsync(answer.FlowId, conversationId);

            return new DialogDto(conversationId, answer.Id, answer.Intent, start?.Question, AnswerTypes.ProcessFlow);
        }

        public IEnumerable<KnowledgeMap> GetDialogFlowsQuery(Func<KnowledgeMap, bool> where)
        {
            return _context.KnowledgeMaps.Where(where);
        }
    }
}
