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
            var question = "��Һ�ѽ";
            var pe = ml.CreatePredictionEngine();
            var result = ml.Predict(pe, question);

            Assert.Equal("�ʺ�", result.Intent);
        }

        private List<QuestionIntent> FakeQuestions() => new List<QuestionIntent> {
            new QuestionIntent{ Question="hi",Intent="�ʺ�"},
            new QuestionIntent{ Question="hello",Intent="�ʺ�"},
            new QuestionIntent{ Question="��",Intent="�ʺ�"},
            new QuestionIntent{ Question="���",Intent="�ʺ�"},
            new QuestionIntent{ Question="���ѽ",Intent="�ʺ�"},
            new QuestionIntent{ Question="����˭",Intent="�ʺ�"},
            new QuestionIntent{ Question="�ܸ��˼�����",Intent="�ʺ�"},
            new QuestionIntent{ Question="���",Intent="���"},
            new QuestionIntent{ Question="�������",Intent="���"},
            new QuestionIntent{ Question="��Ҫ���",Intent="���"},
            new QuestionIntent{ Question="��������",Intent="���"},
            new QuestionIntent{ Question="�����岻���",Intent="���"},
            new QuestionIntent{ Question="�����������",Intent="����"},
            new QuestionIntent{ Question="�������",Intent="����"},
            new QuestionIntent{ Question="����",Intent="����"}
        };
    }
}
