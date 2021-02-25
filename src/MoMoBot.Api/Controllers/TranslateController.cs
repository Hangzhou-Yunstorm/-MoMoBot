using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MoMoBot.Infrastructure;
using Newtonsoft.Json;

namespace MoMoBot.Api.Controllers
{
    [ApiController]
    [Route("/api/translate")]
    public class TranslateController : ControllerBase
    {
        private readonly IEasyCachingProvider _cache;
        private readonly IConfiguration _config;
        public TranslateController(IEasyCachingProvider cache,
            IConfiguration config)
        {
            _config = config;
            _cache = cache;
        }

        [HttpPost("text")]
        public async Task<IActionResult> TranslateText([FromBody]TextTranslateViewModel vm)
        {
            using (var http = new HttpClient())
            {
                http.Timeout = TimeSpan.FromMinutes(1);
                using (var request = new HttpRequestMessage())
                {
                    var body = JsonConvert.SerializeObject(new[] { new { vm.Text } });
                    var key = _config.GetValue("CognitiveServicesKeys:Translate","");

                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri($"https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&to={vm.To}&from={vm.from}");
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", key);

                    var response = await http.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        return Ok(result);
                    }
                    return BadRequest();
                }
            }
        }

        [HttpGet("languages")]
        public async Task<IActionResult> GetLanguages()
        {
            Language languages = null;
            var cacheValue = await _cache.GetAsync<Language>(Constants.CacheKey.Languages);
            if (cacheValue.HasValue)
            {
                languages = cacheValue.Value;
            }
            if (languages == null)
            {
                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(30);
                    http.DefaultRequestHeaders.Add("Accept-Language", "zh-Hans");
                    var response = await http.GetAsync("https://api.cognitive.microsofttranslator.com/languages?api-version=3.0&scope=translation");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        languages = JsonConvert.DeserializeObject<Language>(content);
                        await _cache.SetAsync<Language>(Constants.CacheKey.Languages, languages, TimeSpan.FromDays(3));
                    }
                }
            }
            var translation = languages?.translation?.Select(trans => new { lang = trans.Key, name = trans.Value.Name, nativeName = trans.Value.NativeName });
            return Ok(translation);
        }
    }

    public class TextTranslateViewModel : TranslateViewModel
    {
        [Required]
        [MaxLength(250)]
        public string Text { get; set; }
    }

    public class TranslateViewModel
    {
        [Required]
        [MaxLength(10)]
        public string To { get; set; }
        [MaxLength(10)]
        public string from { get; set; }
    }

    [Serializable]
    public class Language
    {
        public Dictionary<string, Translation> translation { get; set; }

        [Serializable]
        public class Translation
        {
            public string Name { get; set; }
            public string NativeName { get; set; }
            public string Dir { get; set; }
        }
    }
}
