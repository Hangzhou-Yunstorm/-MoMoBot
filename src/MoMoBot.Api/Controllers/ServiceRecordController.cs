using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MoMoBot.Api.ViewModels;
using MoMoBot.Infrastructure.Extensions;
using MoMoBot.Infrastructure.Models;
using MoMoBot.Service.Abstractions;

namespace MoMoBot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServiceRecordController : ControllerBase
    {
        private readonly TokenValidationParameters _validationParameters;
        private readonly IServiceRecordService _recordService;
        public ServiceRecordController(TokenValidationParameters validationParameters,
            IServiceRecordService serviceRecordService)
        {
            _validationParameters = validationParameters;
            _recordService = serviceRecordService;
        }

        [HttpPost("placeonfile")]
        public async Task<IActionResult> PlaceOnFile([FromBody]ServiceRecordModel vm)
        {
            var current = HttpContext.IdentityUser<MoMoBotUser>(_validationParameters);
            if (current != null)
            {
                var record = await _recordService.FindTask(current.Id, vm.Id);
                if (record != null)
                {
                    if (record.IsDone)
                    {
                        return Ok(vm.Id);
                    }
                    record.Remarks = vm.Remarks;
                    record.IsDone = vm.IsDone;
                    record.Title = vm.Title;
                    if (vm.IsDone)
                    {
                        record.RevordCompletionTime = DateTime.Now;
                    }
                    await _recordService.TaskCompletion(record);
                    if (vm.IsDone)
                    {
                        return Ok(vm.Id);
                    }
                    return Ok();
                }
                return BadRequest();
            }
            return Unauthorized();
        }

        [HttpDelete("remove/{id}")]
        public async Task<IActionResult> Remove(long id)
        {
            var current = HttpContext.IdentityUser<MoMoBotUser>(_validationParameters);
            if (current != null)
            {
                await _recordService.DeleteAndSaveChangeAsync(current.Id, id);
                return Ok();
            }
            return Unauthorized();
        }
    }
}
