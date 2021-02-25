using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoMoBot.Core;
using MoMoBot.Core.Schema;
using MoMoBot.Service.Map;

namespace MoMoBot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly KnowledgeMapContext _knowledgeMap;
        public TestController(ILogger<TestController> logger,
            KnowledgeMapContext knowledgeMap)
        {
            _knowledgeMap = knowledgeMap;
            _logger = logger;
        }
        //private readonly DomainPropertyAccessors _propertyAccessors;
        //public TestController(DomainPropertyAccessors propertyAccessors)
        //{
        //    _propertyAccessors = propertyAccessors;
        //}

        //[HttpPost("conversation")]
        //public async Task<IActionResult> Conversation([FromBody]Activity activity)
        //{
        //    using (var context = new TurnContext(activity))
        //    {
        //        var test = await _propertyAccessors.TestModelPropertyAccessor.GetAsync(context, () => new TestModel());
        //        if (string.IsNullOrWhiteSpace(test.Id))
        //        {
        //            await _propertyAccessors.TestModelPropertyAccessor.SetAsync(context, new TestModel { Id = activity.ChannelId });
        //            await _propertyAccessors.SaveStatesAsync(context);
        //            return Ok();
        //        }
        //        return Ok(test);
        //    }
        //}

        [HttpGet("error")]
        public IActionResult Error()
        {
            Convert.ToInt32("abc");
            return Ok();
        }

        [HttpGet("Warning")]
        public IActionResult Warning()
        {
            _logger.LogWarning("警告警告！！！！！");
            return Ok();
        }

        [HttpGet("map")]
        public async Task<IActionResult> Map(string answer, [FromHeader(Name = "x-yc-conversationId")]string conversationId)
        {
            await _knowledgeMap.StartAsync("travel_application_test", conversationId);

            if (_knowledgeMap.HasNext())
            {
                var step = await _knowledgeMap.ContinueAsync(answer, conversationId);
                return Ok(new { _knowledgeMap.Next, step.Question });
            }
            var state = await _knowledgeMap.GetStateAsync(conversationId);
            await _knowledgeMap.DestroyAsync(conversationId);
            return Ok(state);
        }
    }
}
