using MoMoBot.Infrastructure.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MoMoBot.Infrastructure.DTalk;
using MoMoBot.Infrastructure.DTalk.Models;
using Microsoft.Extensions.Options;

namespace MoMoBot.Infrastructure.DTalk
{
    public class DingTalkHelper
    {
        private readonly HttpRequestHelper _http;
        private readonly DTalkConfig _config;
        public DingTalkHelper(HttpRequestHelper http,
            IOptions<DTalkConfig> options)
        {
            _http = http;
            _config = options.Value ?? throw new ArgumentNullException(nameof(_config));
        }

        /// <summary>
        /// 获取钉钉Token
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAccessTokenAsync()
        {
            var url = $"https://oapi.dingtalk.com/gettoken?corpid={_config.CorpId}&corpsecret={_config.CorpSecret}";

            var oat = await _http.GetAsync<ResultModel.AccessTokenModel>(url);

            return oat?.access_token;
        }

        /// <summary>
        /// 获取企业部门列表
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<ResultModel.DepartmentListModel> GetDepartmentsListAsync(string accessToken)
        {
            var url = $"https://oapi.dingtalk.com/department/list?access_token={accessToken}";
            var oat = await _http.GetAsync<ResultModel.DepartmentListModel>(url);
            return oat;
        }

        /// <summary>
        /// 获取JS ticket
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<string> GetJsApiTicketAsync(string accessToken)
        {
            var url = $"https://oapi.dingtalk.com/get_jsapi_ticket?access_token={accessToken}";

            var model = await _http.GetAsync<ResultModel.JsApiTicketModel>(url);

            return model?.ticket;
        }

        /// <summary>
        /// 获取Userid
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<string> GetUserIdAsync(string accessToken, string code)
        {
            string url = $"https://oapi.dingtalk.com/user/getuserinfo?access_token={accessToken}&code={code}";
            var model = await _http.GetAsync<ResultModel.GetUserInfoModel>(url);
            return model?.userid;
        }

        /// <summary>
        /// 获取UserInfo
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ResultModel.Dingding_User> GetUserInfoAsync(string accessToken, string userId)
        {
            var url = $"https://oapi.dingtalk.com/user/get?access_token={accessToken}&userid={userId}";
            var model = await _http.GetAsync<ResultModel.Dingding_User>(url);
            return model;
        }

        /// <summary>
        /// 获取部门信息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResultModel.DepartmentInfo> GetDepartmentInfoByIdAsync(string accessToken, string id)
        {
            var url = $"https://oapi.dingtalk.com/department/get?access_token={accessToken}&id={id}";
            var model = await _http.GetAsync<ResultModel.DepartmentInfo>(url);
            return model;
        }

        /// <summary>
        /// 获取Utc时间戳
        /// </summary>
        /// <returns></returns>
        public long GetUtcTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        /// <summary>
        /// 签名算法 对string1进行sha1签名，得到signature
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<Access_Sdk> GetSdkAsync(string url, string jsApiTicket)
        {
            string noncestr = CommonHelper.GuidTo16String();
            string timestamp = GetUtcTimeStamp().ToString();
            string string1 = "jsapi_ticket=" + jsApiTicket + "&noncestr=" + noncestr + "&timestamp=" + timestamp + "&url=" + url + "";
            string signature = CommonHelper.Sha1(string1);
            Access_Sdk sdk = new Access_Sdk();
            sdk.noncestr = noncestr;
            sdk.timestamp = timestamp;
            sdk.signature = signature;
            return await Task.FromResult(sdk);
        }

        /// <summary>
        /// 获取钉钉配置信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="jsApiTicket"></param>
        /// <returns></returns>
        public async Task<ResultModel.ResultInfo<Dictionary<string, string>>> GetDingdingConfigAsync(string url, string jsApiTicket)
        {
            string _agentId = _config.AgentId;
            string _corpId = _config.CorpId;
            Access_Sdk sdk = await GetSdkAsync(url, jsApiTicket);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("agentId", _agentId);
            dic.Add("corpId", _corpId);
            dic.Add("timeStamp", sdk.timestamp);
            dic.Add("nonceStr", sdk.noncestr);
            dic.Add("signature", sdk.signature);
            var result = new ResultModel.ResultInfo<Dictionary<string, string>>();
            result.Code = ResultModel.ResultCode.Success;
            result.Data = dic;
            result.Message = "ok";
            return await Task.FromResult(result);
        }

        /// <summary>
        /// 发送工作通知
        /// </summary>
        public async Task<DDWorkNoticeSentResult> SendWorkNoticeAsync(DDMessage msg, string userids)
        {
            var access_token = GetAccessTokenAsync();
            // var t = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;

            var list = new List<KeyValuePair<string, string>>();
            list.Add(new KeyValuePair<string, string>("agent_id", _config.AgentId));
            list.Add(new KeyValuePair<string, string>("userid_list", userids));
            list.Add(new KeyValuePair<string, string>("msg", JsonConvert.SerializeObject(msg)));
            var requestContent = new FormUrlEncodedContent(list);
            var result = await _http.PostAsync<DDWorkNoticeSentResult>($"https://oapi.dingtalk.com/topapi/message/corpconversation/asyncsend_v2?access_token={access_token}", requestContent);
            return result;
        }

        /// <summary>
        /// 获取当前钉钉用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="corpId"></param>
        /// <param name="corpSecret"></param>
        /// <returns></returns>
        public async Task<DingDingUser> GetCurrentDDUserInfoAsync(string userId)
        {
            DingDingUser user = null;
            if (!string.IsNullOrEmpty(userId))
            {
                var token = await GetAccessTokenAsync();
                var userInfo = await GetUserInfoAsync(token, userId);
                if (userInfo != null)
                {
                    var depart = await GetDepartmentInfoByIdAsync(token, userInfo.department?.First());
                    user = new DingDingUser
                    {
                        UserId = userInfo.userid,
                        Email = userInfo.email,
                        UserName = userInfo.name,
                        Departments = new[] { depart?.Name },
                        DepartIds = new[] { depart?.Id.ToString() },
                    };
                }
            }

            return user;
        }
    }
}