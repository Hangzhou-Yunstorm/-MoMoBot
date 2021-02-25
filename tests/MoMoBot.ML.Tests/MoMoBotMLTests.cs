using MoMoBot.ML.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace MoMoBot.ML.Tests
{
    public class MoMoBotMLTests
    {
        private readonly MoMoBotML ml;
        public MoMoBotMLTests()
        {
            ml = new MoMoBotML(@"F:\tfs\MoMoBot\MoMoBot.Api\MLModels", "intent_model.zip");
        }

        [Fact]
        public void build_and_train_model_successed()
        {
            var data = FakeQuestions();

            ml.BuildAndTrainModel(data);
        }

        [Fact]
        public void get_question_intent_successed()
        {
            var data = FakeQuestions();

            ml.BuildAndTrainModel(data);
            var question = "大家好呀";
            var pe = ml.CreatePredictionEngine();
            var result = ml.Predict(pe, question);

            Assert.Equal("问候", result.Intent);
        }

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
