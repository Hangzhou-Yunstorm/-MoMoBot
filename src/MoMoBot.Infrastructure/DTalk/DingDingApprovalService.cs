using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MoMoBot.Infrastructure.DTalk.Models;
using Newtonsoft.Json;

namespace MoMoBot.Infrastructure.DTalk
{
    public class DingDingApprovalService
    {
        private readonly HttpClient _httpClient;
        private readonly DTalkConfig _ddConfig;
        public DingDingApprovalService(HttpClient httpClient,
            IOptions<DTalkConfig> options)
        {
            _httpClient = httpClient;
            _ddConfig = options.Value;
        }

        public async Task<bool> LaunchTravelApprovalAsync(TravelApprovalRequestModel data)
        {
            var token = await GetAccessTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                var url = $"https://oapi.dingtalk.com/topapi/processinstance/create?access_token={token}";
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApprovalResult>(json);
                    return result != null && result.errcode == 0;
                }
            }
            return false;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var url = $"https://oapi.dingtalk.com/gettoken?corpid={_ddConfig.CorpId}&corpsecret={_ddConfig.CorpSecret}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<AccessTokenModel>(json);
                return result?.access_token;
            }
            return null;
        }
    }

    class ApprovalResult
    {
        public int errcode { get; set; }
        public string process_instance_id { get; set; }
        public string request_id { get; set; }
    }

    class AccessTokenModel
    {
        public string access_token { get; set; }
        public int errcode { get; set; }
        public string errmsg { get; set; }
    }

    public class TravelApprovalRequestModel
    {
        public string process_code = "PROC-7KYJ2W8W-U3526E7LYGDB0U0IT6GU1-LMG7JYQJ-2";
        public string originator_user_id { get; set; }
        public string dept_id { get; set; }
        public string approvers { get; set; }
        public List<FormComponentValue> form_component_values { get; set; }
    }
    public class FormComponentValue
    {
        public string name { get; set; }
        public string value { get; set; }
    }
}
