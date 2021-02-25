using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoMoBot.Infrastructure.Database;
using MoMoBot.Infrastructure.Models;
using MoMoBot.Service.Abstractions;

namespace MoMoBot.Service.Implements
{
    public class VoiceService
        : IVoiceService
    {
        private readonly MoMoDbContext _dbContext;
        private readonly string _voicesPath;
        private readonly Authentication _authentication;
        private readonly string _abstractPath;
        private readonly HttpClient _http;
        public VoiceService(IHostingEnvironment env, IConfiguration config, MoMoDbContext dbContext, HttpClient http)
        {
            _http = http;
            _dbContext = dbContext;
            var endpoint = config.GetValue("CognitiveServicesKeys:Endpoint", "");
            var key = config.GetValue("CognitiveServicesKeys:Voice", "");
            _authentication = new Authentication($"{endpoint}/sts/v1.0/issueToken", key);
            _voicesPath = Path.Combine("voices", DateTime.Now.ToString("yyyyMMdd"));
            _abstractPath = Path.Combine(env.ContentRootPath, "upload", _voicesPath);
            if (Directory.Exists(_abstractPath) == false)
            {
                Directory.CreateDirectory(_abstractPath);
            }
        }

        public async Task<Guid> CreateAsync(IFormFile file, double duration, string type = "wav")
        {
            if (file != null)
            {
                var id = Guid.NewGuid();

                var filename = $"{id}.wav";
                var path = Path.Combine(_voicesPath, filename);
                var abstrctPath = Path.Combine(_abstractPath, filename);

                using (var writer = new FileStream(abstrctPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    await file.CopyToAsync(writer);
                }
                await _dbContext.Voices.AddAsync(new Voice
                {
                    CreationTime = DateTime.Now,
                    AudioType = type,
                    Id = id,
                    Duration = duration,
                    SavePath = path
                });
                await _dbContext.SaveChangesAsync();
                return id;
            }
            return Guid.Empty;
        }



        public async Task<Voice> GetAsync(Guid id)
        {
            var voice = await _dbContext.Voices.FirstOrDefaultAsync(v => v.Id == id);
            return voice;
        }

        public async Task<string> TTS(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                return await GetOrCreateAsync(text);
            }

            return string.Empty;
        }

        private async Task<string> GetOrCreateAsync(string text)
        {
            var path = string.Empty;
            var voice = await _dbContext.Voices.FirstOrDefaultAsync(v => v.Text.Equals(text, StringComparison.CurrentCultureIgnoreCase));
            if (voice != null)
            {
                path = voice.SavePath;
            }
            else
            {
                var accessToken = await _authentication.FetchTokenAsync();
                var host = "https://southeastasia.tts.speech.microsoft.com/cognitiveservices/v1";
                var body = @"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='zh-CN'>
              <voice name='Microsoft Server Speech Text to Speech Voice (zh-CN, XiaoxiaoNeural)'>" +
                  text + "</voice></speak>";

                using (var request = new HttpRequestMessage())
                {
                    // Set the HTTP method
                    request.Method = HttpMethod.Post;
                    // Construct the URI
                    request.RequestUri = new Uri(host);
                    // Set the content type header
                    request.Content = new StringContent(body, Encoding.UTF8, "application/ssml+xml");
                    // Set additional header, such as Authorization and User-Agent
                    request.Headers.Add("Authorization", "Bearer " + accessToken);
                    request.Headers.Add("Connection", "Keep-Alive");
                    // Update your resource name
                    request.Headers.Add("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1");
                    request.Headers.Add("X-Microsoft-OutputFormat", "riff-24khz-16bit-mono-pcm");
                    // Create a request
                    using (var response = await _http.SendAsync(request).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        // Asynchronously read the response
                        using (var dataStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        {
                            var id = Guid.NewGuid();
                            var filename = $"{id}.wav";
                            path = Path.Combine(_voicesPath, filename);
                            var abstrctPath = Path.Combine(_abstractPath, filename);
                            using (var writer = new FileStream(abstrctPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                            {
                                await dataStream.CopyToAsync(writer);
                            }
                            await _dbContext.Voices.AddAsync(new Voice
                            {
                                Text = text,
                                CreationTime = DateTime.Now,
                                AudioType = "wav",
                                Id = id,
                                Duration = 0d,
                                SavePath = path
                            });
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                }

            }

            return Path.Combine("files", path);
        }
    }

    public class Authentication
    {
        private string subscriptionKey;
        private string tokenFetchUri;

        public Authentication(string tokenFetchUri, string subscriptionKey)
        {
            if (string.IsNullOrWhiteSpace(tokenFetchUri))
            {
                throw new ArgumentNullException(nameof(tokenFetchUri));
            }
            if (string.IsNullOrWhiteSpace(subscriptionKey))
            {
                throw new ArgumentNullException(nameof(subscriptionKey));
            }
            this.tokenFetchUri = tokenFetchUri;
            this.subscriptionKey = subscriptionKey;
        }

        public async Task<string> FetchTokenAsync()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", this.subscriptionKey);
                UriBuilder uriBuilder = new UriBuilder(this.tokenFetchUri);

                var result = await client.PostAsync(uriBuilder.Uri.AbsoluteUri, null).ConfigureAwait(false);
                return await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }
    }
}
