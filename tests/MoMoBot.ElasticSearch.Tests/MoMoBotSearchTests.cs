using MoMoBot.ElasticSearch.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MoMoBot.ElasticSearch.Tests
{
    public class MoMoBotSearchTests
    {
        private readonly MoMoBotSearch _search;
        public MoMoBotSearchTests()
        {
            _search = new MoMoBotSearch("http://localhost:9200");
        }

        [Fact]
        public async Task Create_Index_Successful()
        {
            var suggestions = FakeSuggestions();

            var result = await _search.CreateIndexesAsync(suggestions);

            Assert.True(result);
        }

        [Fact]
        public async Task Get_Suggestion_Successful()
        {
            var question = "查询调休";

            var suggestions = await _search.GetSuggestionsAsync(question);

            Assert.NotNull(suggestions);
        }


        private IList<Question> FakeSuggestions() => new List<Question>
        {
            new Question(Guid.NewGuid().ToString("N"), "查看我的剩余调休时间","查询调休"),
            new Question(Guid.NewGuid().ToString("N"),"查看我的剩余年假","查询年假")
        };
    }
}
