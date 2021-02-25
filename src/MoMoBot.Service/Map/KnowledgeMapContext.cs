using EasyCaching.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoMoBot.Core;
using MoMoBot.Infrastructure.Database;
using MoMoBot.Infrastructure.Models.KnowledgeMapModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoMoBot.Service.Map
{
    public class KnowledgeMapContext
    {
        public const string MAP = "map";
        public const string STEPS = "steps";
        public const string DATA = "data";
        public const string STATE = "state";
        public const string FLOW = "flowid";

        private readonly MoMoDbContext _dbContext;
        private readonly ILogger<KnowledgeMapContext> _logger;
        private readonly IStorage _storage;
        public KnowledgeMapContext(MoMoDbContext dbContext,
            IStorage storage,
            ILogger<KnowledgeMapContext> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            _storage = storage;
        }

        public KnowledgeMap Map { get; private set; }
        public IList<Step> Steps { get; private set; }
        public long? Next { get; private set; }

        public async Task<ContinueResult> StartAsync(string identifier, string conversationId)
        {
            await _storage.WriteAsync(new Dictionary<string, object> { { GetCacheKey(conversationId, FLOW), new ConversationFlow { FlowId = identifier } } });
            await LoadDataAsync(identifier, conversationId);

            return await ContinueAsync("", conversationId);
        }

        public async Task<ContinueResult> ContinueAsync(string answer, string conversationId)
        {
            MoMoBotAssert.NotNull(Steps);

            var isQuestion = true;
            var step = GetCurrentStep();
            if (step != null)
            {
                var result = (string)ExcuteMethod(step.Function, answer);
                // todo : get field's value
                await SetValueAsync(conversationId, step.Key, new FieldItem { Filed = "field", Value = result ?? answer });
                // 分支节点
                if (step.StepType == StepTypes.BranchNode)
                {
                    var next = Steps.FirstOrDefault(s => s.PrevStep == step.Id && s.TriggeredResult == result);
                    if (next != null)
                    {
                        if (next.StepType == StepTypes.RunningNode)
                        {
                            var next2 = Steps.FirstOrDefault(s => s.PrevStep == next.Id);
                            Next = next2?.Id;
                            isQuestion = !(next2?.StepType == StepTypes.BranchNode);
                        }
                        else
                        {
                            throw new Exception("Error!");
                        }

                        step = next;
                    }
                    else
                    {
                        step = Steps.FirstOrDefault(s => s.StepType == StepTypes.EndNode);
                    }
                }
                // 运行节点与开始节点
                else if (step.StepType == StepTypes.StartNode ||
                    step.StepType == StepTypes.RunningNode)
                {
                    var next = Steps.FirstOrDefault(s => s.Id == step.NextStep);
                    if (next?.StepType == StepTypes.EndNode)
                    {
                        isQuestion = false;
                    }
                    Next = next?.Id;
                }
                // 结束节点
                else
                {
                    Next = 0;//Steps.FirstOrDefault(s => s.StepType == StepTypes.EndNode)?.Id;
                }
            }
            await _storage.WriteAsync(new Dictionary<string, object> { { GetCacheKey(conversationId, DATA), new ConversationData { Next = Next ?? 0 } } });

            return step == null ? null : new ContinueResult
            {
                IsQuestion = isQuestion,
                Question = step.Question
            };
        }

        public async Task LoadFromChacheAsync(string conversationId)
        {
            var key = GetCacheKey(conversationId, FLOW);
            var flow = await _storage.ReadAsync(key, () => new ConversationFlow { FlowId = "" });
            await LoadDataAsync(flow.FlowId, conversationId);
        }

        public async Task DestroyAsync(string conversationId)
        {
            await _storage.DeleteAsync(new[] { GetCacheKey(conversationId, FLOW), GetCacheKey(conversationId, STATE), GetCacheKey(conversationId, DATA) });
        }

        public bool HasNext() => Next != null && Next.HasValue && Next.Value > 0;

        public async Task<Dictionary<string, object>> GetStateAsync(string conversationId)
        {
            return await _storage.ReadAsync(GetCacheKey(conversationId, STATE), () => new Dictionary<string, object>());
        }

        private async Task LoadDataAsync(string identifier, string conversationId)
        {
            if (string.IsNullOrEmpty(identifier) || string.IsNullOrEmpty(conversationId))
            {
                return;
            }
            Map = await _storage.ReadAsync($"{MAP}:{identifier}", () => _dbContext.KnowledgeMaps.FirstOrDefault(m => m.Identifier == identifier));

            MoMoBotAssert.NotNull(Map);

            Steps = await _storage.ReadAsync($"{MAP}:{identifier}:{STEPS}", () => _dbContext.Steps
                .Where(s => s.MapId == Map.Id)
                .Select(s => new Step
                {
                    Id = s.Id,
                    Function = s.Function,
                    TriggeredResult = s.TriggeredResult,
                    StepType = s.StepType,
                    ResultType = s.ResultType,
                    Key = s.Key,
                    NextStep = s.NextStep,
                    PrevStep = s.PrevStep,
                    Question = s.Question
                }).ToList());

            var data = await _storage.ReadAsync(GetCacheKey(conversationId, DATA), () => new ConversationData { Next = Steps.FirstOrDefault(s => s.StepType == StepTypes.StartNode)?.Id });

            if (data != null && data.HasNext)
            {
                if (data.Next.Value > 0)
                {
                    Next = Steps.FirstOrDefault(s => s.Id == data.Next.Value)?.Id;
                }
                else if (data.Next.Value == 0)
                {
                    Next = null;
                }
            }
            else
            {
                Next = Steps.FirstOrDefault(s => s.StepType == StepTypes.StartNode)?.Id;
            }
        }

        private async Task SetValueAsync(string conversationId, string field, object value)
        {
            if (string.IsNullOrEmpty(field))
            {
                return;
            }
            var key = GetCacheKey(conversationId, STATE);
            var dic = await _storage.ReadAsync(key, () => new Dictionary<string, object>());
            if (dic.ContainsKey(field))
            {
                dic[field] = value;
            }
            else
            {
                dic.Add(field, value);
            }
            await _storage.WriteAsync(new Dictionary<string, object> { { key, dic } });
        }

        private Step GetCurrentStep()
        {
            Step step = null;
            if (Next.HasValue)
            {
                step = Steps.FirstOrDefault(s => s.Id == Next.Value);
            }
            else
            {
                step = Steps.FirstOrDefault(s => s.StepType == StepTypes.EndNode);
            }
            return step;
        }

        private object ExcuteMethod(string methodFullName, string param)
        {
            if (!string.IsNullOrEmpty(methodFullName))
            {
                var lastIndex = methodFullName.AsSpan().LastIndexOf('.');
                if (lastIndex >= 0)
                {
                    var methodName = methodFullName.AsSpan().Slice(lastIndex + 1, methodFullName.Length - lastIndex - 1);
                    var className = methodFullName.AsSpan().Slice(0, lastIndex);

                    var type = Type.GetType(className.ToString());
                    if (type != null)
                    {
                        var obj = Activator.CreateInstance(type);
                        var method = type.GetMethod(methodName.ToString(), new Type[] { typeof(string) });
                        if (method != null)
                        {
                            var parameters = new[] { param };
                            var result = method.Invoke(obj, parameters);
                            return result;
                        }
                    }
                }
            }
            return null;
        }

        private string GetCacheKey(string conversationId, string key)
        {
            if (string.IsNullOrEmpty(conversationId))
            {
                throw new ArgumentNullException(nameof(conversationId));
            }
            return $"{conversationId}:{key}";
        }
    }

    public class TestClass
    {
        public string YesOrNo(string answer)
        {
            return (answer == "yes").ToString();
        }
    }

    public class ContinueResult
    {
        public string Question { get; set; }
        public bool IsQuestion { get; set; } = true;
    }

    [Serializable]
    public class Step
    {
        public long Id { get; set; }
        public long? PrevStep { get; set; }
        public long? NextStep { get; set; }
        public StepTypes StepType { get; set; }
        public string Question { get; set; }
        public string Key { get; set; }
        public string ResultType { get; set; }
        public string TriggeredResult { get; set; }
        public string Function { get; set; }
    }

    [Serializable]
    public class ConversationFlow
    {
        public string FlowId { get; set; }
    }

    [Serializable]
    public class ConversationData
    {
        public long? Next { get; set; }

        [JsonIgnore]
        public bool HasNext => Next.HasValue;
    }
    public class FieldItem
    {
        public string Filed { get; set; }
        public string Value { get; set; }
    }
}
