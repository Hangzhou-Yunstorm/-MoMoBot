using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoMoBot.Api.ViewModels;
using MoMoBot.Service.Abstractions;
using System.Threading.Tasks;

namespace MoMoBot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SettingsController : ControllerBase
    {
        private readonly IAppSettings _appSettings;
        public SettingsController(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        [HttpPost("clear-cache")]
        public IActionResult ClearCache()
        {
            _appSettings.ClearCacheAsync();
            return Ok();
        }

        [HttpPost("set")]
        public async Task<IActionResult> SetValue([FromBody]InputSettingViewModel vm)
        {
            await _appSettings.SetAsync(vm.Key, vm.Value);
            return Ok();
        }

        [HttpGet("value")]
        public async Task<IActionResult> GetValue(string key)
        {
            if (string.IsNullOrWhiteSpace(key) == false)
            {
                var value = await _appSettings.GetAsync(key);
                return Ok(value);
            }
            return Ok();
        }

        [HttpGet("all-values")]
        public async Task<IActionResult> GetAllValues()
        {
            var values = await _appSettings.GetAllAsync();
            return Ok(values);
        }
    }
}
