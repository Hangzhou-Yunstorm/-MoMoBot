using Hangfire;
using Microsoft.AspNetCore.Mvc;
using MoMoBot.ML;
using MoMoBot.ML.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoMoBot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MLController : ControllerBase
    {
        private readonly MoMoBotML _ml;
        public MLController(MoMoBotML ml)
        {
            _ml = ml;
        }

        [HttpPost("train")]
        public IActionResult Train()
        {
            RecurringJob.Trigger("momobot_ml_training");
            return Ok("训练成功");
        }

        [HttpPost("predict")]
        public IActionResult Predict([FromBody]PredictViewModel vm)
        {
            var predict = _ml.CreatePredictionEngine();
            var result = _ml.Predict(predict, vm.Question);
            return Ok(result);
        }
    }

    public class PredictViewModel
    {
        [Required]
        public string Question { get; set; }
    }
}
