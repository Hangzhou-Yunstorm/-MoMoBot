using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MoMoBot.Infrastructure.Models;

namespace MoMoBot.Service.Abstractions
{
    public interface IVoiceService
    {
        Task<Guid> CreateAsync(IFormFile file, double duration, string type = "wav");
        Task<Voice> GetAsync(Guid id);
        /// <summary>
        /// 文字转语音
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        Task<string> TTS(string text);
    }
}
