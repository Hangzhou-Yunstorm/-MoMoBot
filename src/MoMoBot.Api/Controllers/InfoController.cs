using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MoMoBot.Infrastructure.DTalk;
using MoMoBot.Infrastructure.DTalk.Models;

namespace MoMoBot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DingTalkHelper _ddHelper;
        private readonly DTalkConfig _ddConfig;
        public InfoController(IConfiguration configuration,
            DingTalkHelper ddHelper,
            IOptions<DTalkConfig> options)
        {
            _ddHelper = ddHelper;
            _configuration = configuration;
            _ddConfig = options.Value;
        }

        [HttpGet("dd")]
        public async Task<IActionResult> GetDDConfig()
        {
            //获取Token
            var token = await _ddHelper.GetAccessTokenAsync();
            //HttpContext.Session.SetString("Token", _token);
            //获取部门
            //string _depart = DingTalkHelper.GetDepartmentsList(_token);
            //获取JsApiTicket
            var ticket = await _ddHelper.GetJsApiTicketAsync(token);
            //HttpContext.Session.SetString("Ticket", _ticket);
            //获取url
            string url = _configuration.GetValue("ReactClientUrl", "");//"http://" + HttpContext.Request.Host.ToString();
            //生成config信息
            var _ddconfig = await _ddHelper.GetDingdingConfigAsync(url, ticket);
            if (_ddconfig.Code == ResultModel.ResultCode.Success)
            {
                Dictionary<string, string> dic = _ddconfig.Data;

                if (dic != null)
                {
                    return Ok(new { token, ticket, _ddConfig.CorpId, url, agentId = dic["agentId"], timeStamp = dic["timeStamp"], nonceStr = dic["nonceStr"], signature = dic["signature"] });
                }
            }

            return Ok();
        }

        /// <summary>
        /// 根据code和token获取用户信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("user")]
        public async Task<IActionResult> GetUserInfo(string code, string token)
        {
            var _userid = await _ddHelper.GetUserIdAsync(token, code);
            var userInfo = await _ddHelper.GetUserInfoAsync(token, _userid);
            ResultModel.ResultInfo<object> result = new ResultModel.ResultInfo<object>();
            result.Data = userInfo;
            result.Code = ResultModel.ResultCode.Success;
            result.Message = "免登录，获取个人信息成功！";
            return Ok(result);
        }
    }
}
