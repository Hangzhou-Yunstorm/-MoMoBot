using MoMoBot.ElasticSearch.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoMoBot.ElasticSearch
{
    public class MoMoBotSearch
    {
        private readonly IElasticClient _client;
        public MoMoBotSearch(string connectionString)
        {
            var settings = new ConnectionSettings(
                new Uri(connectionString)
            ).PrettyJson()
            .DisableDirectStreaming()
            .OnRequestCompleted(callDetails =>
            {
                if (callDetails.RequestBodyInBytes != null)
                {
                    Console.WriteLine(
                        $"{callDetails.HttpMethod} {callDetails.Uri} \n" +
                        $"{Encoding.UTF8.GetString(callDetails.RequestBodyInBytes)}");
                }
                else
                {
                    Console.WriteLine($"{callDetails.HttpMethod} {callDetails.Uri}");
                }

                Console.WriteLine();

                // log out the response
                if (callDetails.ResponseBodyInBytes != null)
                {
                    Console.WriteLine($"Status: {callDetails.HttpStatusCode}\n" +
                             $"{Encoding.UTF8.GetString(callDetails.ResponseBodyInBytes)}\n" +
                             $"{new string('-', 30)}\n");
                }
                else
                {
                    Console.WriteLine($"Status: {callDetails.HttpStatusCode}\n" +
                             $"{new string('-', 30)}\n");
                }
            })
            .DefaultIndex(Constants.IndexName);

            _client = new ElasticClient(settings);

            _client.Indices.Create(Constants.IndexName, c => c
                .Map<Question>(m => m
                    .AutoMap()
                    .Properties(ps => ps
                        .Text(t => t
                            .Name(n => n.Content)
                            .Fields(fs => fs
                                .Text(_t => _t
                                    .Name("chinese_field")
                                    .Analyzer(Constants.ChineseAnalyser)
                                )
                            )
                        )
                    )
                )
            );
        }

        public async Task<bool> CreateIndexAsync(Question intent)
        {
            var response = await _client.IndexAsync(intent, i => i.Index(Constants.IndexName));

            return response.IsValid;
        }

        public async Task<bool> CreateIndexesAsync(ICollection<Question> intents)
        {
            if (intents?.Count <= 0)
            {
                return false;
            }
            var response = await _client.IndexManyAsync(intents);

            return response.IsValid;
        }

        public async Task<IEnumerable<Suggestion>> GetSuggestionsAsync(string query, int size = 3)
        {
            var response = await _client.SearchAsync<Question>(s => s
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(new Field[] {
                            new Field("content"),
                            new Field("content.chinese_field"),
                        })
                        .Query(query)
                    )
                )
                .Take(size)
            );

            if (response.IsValid)
            {
                var suggestions = from hit in response.Hits
                                  select new Suggestion
                                  {
                                      Id = hit.Id,
                                      Intent = hit.Source.Intent,
                                      Text = hit.Source.Text,
                                      Score = hit.Score
                                  };

                return suggestions;
            }

            return default;
        }
    }
}
