using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace MoMoBot.Infrastructure.DTalk
{
    public class HttpRequestHelper
    {
        private readonly HttpClient _http;
        private readonly ILogger<HttpRequestHelper> _logger;
        public HttpRequestHelper(HttpClient http,
            ILogger<HttpRequestHelper> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<T> GetAsync<T>(string url)
        {
            try
            {
                var response = await _http.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(json);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
            return default;
        }

        public async Task<T> PostAsync<T>(string url, HttpContent content = null)
        {
            try
            {
                var response = await _http.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(json);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return default;
        }
    }
}