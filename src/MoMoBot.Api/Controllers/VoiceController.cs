using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoMoBot.Service.Abstractions;

namespace MoMoBot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class VoiceController : ControllerBase
    {
        private readonly IHostingEnvironment _env;
        private readonly IVoiceService _voiceService;
        public VoiceController(IHostingEnvironment env,
            IVoiceService voiceService)
        {
            _voiceService = voiceService;
            _env = env;

        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm]VoiceUploadViewModel vm)
        {
            if (vm.file?.Length > 0)
            {
                var id = await _voiceService.CreateAsync(vm.file, vm.Duration);
                return Created("/api/voice", new { id });
            }
            return BadRequest("没有接收到音频文件");
        }

        [HttpGet("tts")]
        public async Task<IActionResult> ConvertToVoice([Required]string text)
        {
            var url = await _voiceService.TTS(text);
            return Ok(url);
        }


    }

    public class VoiceUploadViewModel
    {
        [Required]
        public IFormFile file { get; set; }
        public double Duration { get; set; }
    }
}
