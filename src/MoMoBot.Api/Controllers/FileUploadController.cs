using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoMoBot.Api.ViewModels;

namespace MoMoBot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        readonly string[] pictureFormatArray = { "png", "jpg", "jpeg", "bmp", "gif", "ico", "PNG", "JPG", "JPEG", "BMP", "GIF", "ICO" };
        private readonly IHostingEnvironment _env;
        public FileController(IHostingEnvironment env)
        {
            _env = env;
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromForm]IFormFile file)
        {
            if (file != null)
            {
                var result = await SaveFileHandler(file);
                return Ok(result);
            }
            return BadRequest();
        }

        private async Task<string> SaveFileHandler(IFormFile file)
        {
            var requestPath = Path.Combine("image", DateTime.Now.ToString("yyyyMMdd"));
            var filePath = Path.Combine(_env.ContentRootPath, "upload", requestPath);
            var ext = file.FileName.Split('.', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            if (string.IsNullOrWhiteSpace(ext) == false)
            {
                if (IsImageFile(ext))
                {


                    if (Directory.Exists(filePath) == false)
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    var filename = $"{Guid.NewGuid()}.{ext}";
                    using (var writer = new FileStream(Path.Combine(filePath, filename), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        await file.CopyToAsync(writer);
                        return Path.Combine("files", requestPath, filename);
                    }
                }
            }

            return string.Empty;
        }


        private bool IsImageFile(string ext) => pictureFormatArray.Any(f => f == ext);
    }
}
