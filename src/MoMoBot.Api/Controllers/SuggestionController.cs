using Microsoft.AspNetCore.Mvc;
using MoMoBot.ElasticSearch;
using MoMoBot.ElasticSearch.Models;
using MoMoBot.Service.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.Controllers
{
    [ApiController]
    [Route("/api/suggestion")]
    public class SuggestionController : ControllerBase
    {
        private readonly IAnswerService _answerService;
        private readonly MoMoBotSearch _search;
        public SuggestionController(MoMoBotSearch search, IAnswerService answerService)
        {
            _search = search;
            _answerService = answerService;
        }

        [HttpGet]
        public async Task<IEnumerable<Suggestion>> Get(string question)
        {
            var suggestions = await _search.GetSuggestionsAsync(question);

            return suggestions;
        }

        [HttpGet, Route("write")]
        public async Task Write()
        {
            var answers = await _answerService.GetAllQandAsAsync();

            var questions = answers?.Select(a => new Question(a.Id.ToString("N"), a.Answer, a.Intent)).ToList();

            await _search.CreateIndexesAsync(questions);
        }
    }
}
