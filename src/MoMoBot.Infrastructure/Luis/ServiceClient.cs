using MoMoBot.Infrastructure.Luis.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MoMoBot.Infrastructure.Luis
{
    public class ServiceClient
    {
        private readonly string _baseUrl;
        private readonly string _subscriptionKey;
        private readonly HttpClient _http;

        public ServiceClient(string subscriptionKey, Location location, HttpClient http)
        {
            _http = http;
            _subscriptionKey = subscriptionKey;
            _baseUrl = $"https://{location.ToString().ToLower()}.api.cognitive.microsoft.com/luis/api/v2.0";
            _http.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
        }

        protected async Task<HttpResponseMessage> Get(string path)
        {
            return await _http.GetAsync($"{_baseUrl}{path}");
        }

        protected async Task<string> Post(string path)
        {
            var response = await _http.PostAsync($"{_baseUrl}{path}", null);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return responseContent;
            else
            {
                var exception = JsonConvert.DeserializeObject<ServiceException>(responseContent);
                throw new Exception($"{exception.Error?.Message ?? exception.Message}");
            }
        }

        protected async Task<string> Post<TRequest>(string path, TRequest requestBody)
        {
            using (var content = new ByteArrayContent(GetByteData(requestBody)))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await _http.PostAsync($"{_baseUrl}{path}", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    return responseContent;
                else
                {
                    var exception = JsonConvert.DeserializeObject<ServiceException>(responseContent);
                    throw new Exception($"{exception.Error?.Message ?? exception.Message}");
                }
            }
        }

        protected async Task Put<TRequest>(string path, TRequest requestBody)
        {
            using (var content = new ByteArrayContent(GetByteData(requestBody)))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await _http.PutAsync($"{_baseUrl}{path}", content);
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var exception = JsonConvert.DeserializeObject<ServiceException>(responseContent);
                    throw new Exception($"{exception.Error?.Message ?? exception.Message}");
                }
            }
        }

        protected async Task Delete(string path)
        {
            var response = await _http.DeleteAsync($"{_baseUrl}{path}");
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var exception = JsonConvert.DeserializeObject<ServiceException>(responseContent);
                throw new Exception($"{exception.Error?.Message ?? exception.Message}");
            }
        }

        private byte[] GetByteData<TRequest>(TRequest requestBody)
        {
            var settings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var body = JsonConvert.SerializeObject(requestBody, settings);
            return Encoding.UTF8.GetBytes(body);
        }
    }
}
