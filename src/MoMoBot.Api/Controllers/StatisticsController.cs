using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoMoBot.Service.Abstractions;

namespace MoMoBot.Api.Controllers
{
    [Route("api/statistics")]
    [ApiController]
    [Authorize]
    public class StatisticsController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IHotIntentService _hotIntentService;
        public StatisticsController(IFeedbackService feedbackService,
            IHotIntentService hotIntentService)
        {
            _feedbackService = feedbackService;
            _hotIntentService = hotIntentService;
        }

        /// <summary>
        /// 反馈统计
        /// </summary>
        /// <returns></returns>
        [HttpGet("feedback")]
        public async Task<IActionResult> FeedbackStatistical()
        {
            var pieModel = await _feedbackService.GetFeedbackPieAsync();
            return Ok(new[] {
                new { x= "差评", y=pieModel.DissatisfiedCount},
                new { x= "一般", y=pieModel.CommonlyCount},
                new { x= "满意", y=pieModel.SatisfiedCount},
            });
        }

        /// <summary>
        /// 热门意图
        /// </summary>
        /// <returns></returns>
        [HttpGet("popular-intents")]
        public async Task<IActionResult> PopularIntents()
        {
            var data = await _hotIntentService.GetHotIntentBarTopTen();
            return Ok(data);
        }

        /// <summary>
        /// 好评意图
        /// </summary>
        /// <returns></returns>
        [HttpGet("praisefeedback")]
        public async Task<IActionResult> PraiseFeedback()
        {
            var praise = await _feedbackService.FeedbackStatistics(1);
            
            return Ok(praise);
        }

        /// <summary>
        /// 差评意图
        /// </summary>
        /// <returns></returns>
        [HttpGet("badfeedback")]
        public async Task<IActionResult> BadFeedback()
        {
            var bad = await _feedbackService.FeedbackStatistics(3);
            return Ok(bad);
        }

    }
}
