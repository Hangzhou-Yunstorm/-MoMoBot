using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoMoBot.Api.ViewModels;
using MoMoBot.Service.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    public class SystemController : ControllerBase
    {
        private readonly ISystemLogReader _logReader;
        public SystemController(ISystemLogReader logReader)
        {
            _logReader = logReader;
        }

        [HttpPost("logs")]
        public async Task<IActionResult> Logs([FromBody]LogQueryViewModel vm)
        {
            if (vm != null)
            {
                var offset = (vm.PageIndex - 1) * vm.PageSize;
                var result = await _logReader.GetLogsAsync(vm.Levels, vm.Start, vm.End, offset, vm.PageSize);
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpGet("error-count")]
        public async Task<IActionResult> ErrorCount()
        {
            var count = await _logReader.GetErrorCountAsync();
            return Ok(count);
        }
    }
}
