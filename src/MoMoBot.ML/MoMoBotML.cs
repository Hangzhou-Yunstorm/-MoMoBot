using Microsoft.ML;
using Microsoft.ML.Data;
using MoMoBot.ML.Exceptions;
using MoMoBot.ML.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MoMoBot.ML
{
    public class MoMoBotML
    {
        private readonly MLContext _mlContext;
        private readonly string _modelPath;
        public MoMoBotML(string modelPath, string fileName)
        {
            _modelPath = Path.Combine(modelPath, fileName);
            _mlContext = new MLContext();
            if (Directory.Exists(modelPath) == false)
            {
                Directory.CreateDirectory(modelPath);
            }
        }

        /// <summary>
        /// 编译并训练
        /// </summary>
        /// <param name="questions"></param>
        public void BuildAndTrainModel(IEnumerable<QuestionIntent> questions)
        {
            var trainingDataView = _mlContext.Data.LoadFromEnumerable(questions);

            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "Label", inputColumnName: nameof(QuestionIntent.Intent))
                .Append(_mlContext.Transforms.Text.FeaturizeText(outputColumnName: "QuestionFeaturized", inputColumnName: nameof(QuestionIntent.Question)))
                .AppendCacheCheckpoint(_mlContext);

            var transformer = pipeline.Fit(trainingDataView);
            var transformedData = transformer.Transform(trainingDataView);

            var averagedPerceptronBinaryTrainer = _mlContext.BinaryClassification.Trainers.AveragedPerceptron("Label", "QuestionFeaturized", numberOfIterations: 5);
            var trainer = _mlContext.MulticlassClassification.Trainers.OneVersusAll(averagedPerceptronBinaryTrainer);

            // 编译
            var trainingPipeline = pipeline.Append(trainer)
                    .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            // 评估（交叉）
            _mlContext.MulticlassClassification.CrossValidate(data: trainingDataView, estimator: trainingPipeline, numberOfFolds: 6, labelColumnName: "Label");

            // 训练
            var trainedModel = trainingPipeline.Fit(trainingDataView);

            _mlContext.Model.Save(trainedModel, trainingDataView.Schema, _modelPath);
        }

        public PredictionEngine<QuestionIntent, QuestionIntentPrediction> CreatePredictionEngine()
        {
            if (File.Exists(_modelPath) == false)
            {
                throw new MoMoBotMLException("必须先训练模型！");
            }
            //Load model from file
            var trainedModel = _mlContext.Model.Load(_modelPath, out var modelInputSchema);

            return _mlContext.Model.CreatePredictionEngine<QuestionIntent, QuestionIntentPrediction>(trainedModel);
        }

        /// <summary>
        /// 预测
        /// </summary>
        /// <param name="predictionEngine"></param>
        /// <param name="question"></param>
        /// <returns></returns>
        public PredictResult Predict(PredictionEngine<QuestionIntent, QuestionIntentPrediction> predictionEngine, string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                throw new ArgumentNullException(nameof(question));
            }

            VBuffer<ReadOnlyMemory<char>> slotNames = default;
            predictionEngine.OutputSchema[nameof(QuestionIntentPrediction.Score)].GetSlotNames(ref slotNames);

            var prediction = predictionEngine.Predict(new QuestionIntent { Question = question });
            var scores = prediction.Score;
            var max = scores.Max();
            var index = scores.AsSpan().IndexOf(max);
            var topIntent = slotNames.GetItemOrDefault(index);
            var length = scores.Length;
            var result = new PredictResult(question, topIntent.ToString(), max);

            for (var i = 0; i < length; i++)
            {
                if (i != index)
                {
                    result.AddPredict(slotNames.GetItemOrDefault(i).ToString(), scores[i]);
                }
            }

            return result;
        }

        private string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(MoMoBotML).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }
    }
}
