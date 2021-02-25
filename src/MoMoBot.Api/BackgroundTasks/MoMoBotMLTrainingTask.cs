using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoMoBot.Infrastructure.Database;
using MoMoBot.ML;
using MoMoBot.ML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.BackgroundTasks
{
    public class MoMoBotMLTrainingTask
    {
        private readonly MoMoBotML _mlContext;
        private readonly MoMoDbContext _dbContext;
        private readonly ILogger<MoMoBotMLTrainingTask> _logger;
        public MoMoBotMLTrainingTask(MoMoBotML mlContext,
            MoMoDbContext dbContext,
            ILogger<MoMoBotMLTrainingTask> logger)
        {
            _mlContext = mlContext;
            _dbContext = dbContext;
            _logger = logger;
        }

        public void Train()
        {
            _logger.LogInformation("开始【MoMoBotMLTrainingTask】任务");

            // 判断是否需要有更新然后获取训练数据
            var isUpdated = NeedToUpdate();
            if (isUpdated)
            {
                var questions = GetTrainingMetadataSet();
                if (questions?.Count() > 0)
                {
                    _mlContext.BuildAndTrainModel(questions);
                    _logger.LogInformation("训练成功！");
                }
            }
        }

        private IEnumerable<QuestionIntent> GetTrainingMetadataSet() =>
            _dbContext.MetadataSet
                .AsNoTracking()
                .Where(m => string.IsNullOrWhiteSpace(m.Intent) && string.IsNullOrWhiteSpace(m.Question))
                .Select(m => new QuestionIntent { Intent = m.Intent, Question = m.Question })
                .OrderBy(m => m.Intent)
                .AsEnumerable();

        private bool NeedToUpdate() => true;

        private List<QuestionIntent> FakeQuestions() => new List<QuestionIntent> {
            new QuestionIntent{ Question="hi",Intent="问候"},
            new QuestionIntent{ Question="hello",Intent="问候"},
            new QuestionIntent{ Question="嗨",Intent="问候"},
            new QuestionIntent{ Question="你好",Intent="问候"},
            new QuestionIntent{ Question="你好呀",Intent="问候"},
            new QuestionIntent{ Question="你是谁",Intent="问候"},
            new QuestionIntent{ Question="很高兴见到你",Intent="问候"},
            new QuestionIntent{ Question="请假",Intent="请假"},
            new QuestionIntent{ Question="我想请假",Intent="请假"},
            new QuestionIntent{ Question="我要请假",Intent="请假"},
            new QuestionIntent{ Question="我生病了",Intent="请假"},
            new QuestionIntent{ Question="我身体不舒服",Intent="请假"},
            new QuestionIntent{ Question="今天天气真好",Intent="闲聊"},
            new QuestionIntent{ Question="天气真好",Intent="闲聊"},
            new QuestionIntent{ Question="哈哈",Intent="闲聊"}
        };
    }
}
