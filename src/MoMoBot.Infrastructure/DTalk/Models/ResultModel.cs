using Newtonsoft.Json;

namespace MoMoBot.Infrastructure.DTalk
{
    public class Access_Sdk
    {
        public string signature { get; set; }
        public string noncestr { get; set; }
        public string timestamp { get; set; }
    }

    public class ResultModel
    {
        public class AccessTokenModel
        {
            public string access_token { get; set; }
            public int errcode { get; set; }
            public string errmsg { get; set; }
        }
        public class DepartmentListModel
        {

            [JsonProperty("errmsg")]
            public string Errmsg { get; set; }

            [JsonProperty("department")]
            public Department[] Department { get; set; }

            [JsonProperty("errcode")]
            public int Errcode { get; set; }
        }
        public class Department
        {

            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("createDeptGroup")]
            public bool CreateDeptGroup { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("autoAddUser")]
            public bool AutoAddUser { get; set; }

            [JsonProperty("parentid")]
            public int? Parentid { get; set; }
        }
        public class JsApiTicketModel
        {
            public int errcode { get; set; }
            public string errmsg { get; set; }
            public string ticket { get; set; }
            public string expires_in { get; set; }
        }
        public class GetUserInfoModel
        {

            public string errmsg { get; set; }
            public string userid { get; set; }
            public string deviceId { get; set; }
            public bool is_sys { get; set; }
            public int sys_level { get; set; }
        }
        public class ResultInfo<T>
        {
            /// <summary>
            /// 错误代码
            /// </summary>
            public ResultCode Code { get; set; }

            /// <summary>
            /// 信息提示
            /// </summary>
            public string Message { get; set; }


            /// <summary>
            /// 数据集结果
            /// </summary>
            public T Data { get; set; }
        }
        public enum ResultCode
        {
            /// <summary>
            /// 成功
            /// </summary>
            Success = 0,

            /// <summary>
            /// 信息错误
            /// </summary>
            InfoError = 1,

            /// <summary>
            /// 缺少参数
            /// </summary>
            ParamError = 10000
        }
        public class Dingding_User
        {
            public string errcode;
            public string errmsg;
            public string userid;
            /// <summary>
            /// 成员名称
            /// </summary>
            public string name;
            /// <summary>
            /// 分机号（仅限企业内部开发调用）
            /// </summary>
            public string tel;
            /// <summary>
            /// 办公地点（ISV不可见）
            /// </summary>
            public string workPlace;
            /// <summary>
            /// 备注（ISV不可见）
            /// </summary>
            public string remark;
            /// <summary>
            /// 手机号码（ISV不可见）
            /// </summary>
            public string mobile;
            /// <summary>
            /// 员工的电子邮箱（ISV不可见）
            /// </summary>
            public string email;
            /// <summary>
            /// 员工的企业邮箱，如果员工已经开通了企业邮箱，接口会返回，否则不会返回（ISV不可见）
            /// </summary>
            public string orgEmail;
            /// <summary>
            /// 是否已经激活, true表示已激活, false表示未激活
            /// </summary>
            public bool active;
            /// <summary>
            /// 在对应的部门中的排序, Map结构的json字符串, key是部门的Id, value是人员在这个部门的排序值
            /// </summary>
            public string orderInDepts;
            /// <summary>
            /// 是否为企业的管理员, true表示是, false表示不是
            /// </summary>
            public bool isAdmin;
            /// <summary>
            /// 是否为企业的老板, true表示是, false表示不是（【设置负责人】：主管理员登陆钉钉手机客户端 -【通讯录】-【企业名后面的管理】-【企业通讯录】-【负责人设置】进行添加则可。）
            /// </summary>
            public bool isBoss;
            /// <summary>
            /// 钉钉Id,在钉钉全局范围内标识用户的身份（不可修改
            /// </summary>
            public string dingId;
            public string unionid;
            /// <summary>
            /// 在对应的部门中是否为主管, Map结构的json字符串, key是部门的Id, value是人员在这个部门中是否为主管, true表示是, false表示不是
            /// </summary>
            public string isLeaderInDepts;
            /// <summary>
            /// 是否号码隐藏, true表示隐藏, false表示不隐藏
            /// </summary>
            public bool isHide;
            /// <summary>
            /// 成员所属部门id列表
            /// </summary>
            public string[] department;
            /// <summary>
            /// 职位信息
            /// </summary>
            public string position;
            /// <summary>
            /// 头像url
            /// </summary>
            public string avatar;

            /// <summary>
            /// 入职时间
            /// </summary>
            public string hiredDate;
            /// <summary>
            /// 员工工号
            /// </summary>
            public string jobnumber;
            /// <summary>
            /// 扩展属性，可以设置多种属性(但手机上最多只能显示10个扩展属性，具体显示哪些属性，请到OA管理后台->设置->通讯录信息设置和OA管理后台->设置->手机端显示信息设置)
            /// </summary>
            public string extattr;

            ///// <summary>
            ///// 角色信息（ISV不可见），json数组格式
            ///// </summary>
            //public Roles roles;

            /// <summary>
            /// 手机号码区号
            /// </summary>
            public string stateCode;

            /// <summary>
            /// 是否是高管
            /// </summary>
            public string isSenior;
        }
        public class Roles
        {
            /// <summary>
            /// 角色id（ISV不可见）
            /// </summary>
            public string id;
            /// <summary>
            ///角色名称（ISV不可见）
            /// </summary>
            public string name;
            /// <summary>
            /// 角色分组名称（ISV不可见）
            /// </summary>
            public string groupName;

            /// <summary>
            /// 
            /// </summary>
            public string type;
        }

        public class DepartmentInfo
        {
            [JsonProperty("errcode")]
            public int Errcode { get; set; }

            [JsonProperty("errmsg")]
            public string Errmsg { get; set; }

            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("order")]
            public int Order { get; set; }

            [JsonProperty("parentid")]
            public int Parentid { get; set; }

            [JsonProperty("createDeptGroup")]
            public bool CreateDeptGroup { get; set; }

            [JsonProperty("autoAddUser")]
            public bool AutoAddUser { get; set; }

            [JsonProperty("deptHiding")]
            public bool DeptHiding { get; set; }

            [JsonProperty("deptPerimits")]
            public string DeptPerimits { get; set; }

            [JsonProperty("userPerimits")]
            public string UserPerimits { get; set; }

            [JsonProperty("outerDept")]
            public bool OuterDept { get; set; }

            [JsonProperty("outerPermitDepts")]
            public string OuterPermitDepts { get; set; }

            [JsonProperty("outerPermitUsers")]
            public string OuterPermitUsers { get; set; }

            [JsonProperty("orgDeptOwner")]
            public string OrgDeptOwner { get; set; }

            [JsonProperty("deptManagerUseridList")]
            public string DeptManagerUseridList { get; set; }

            [JsonProperty("sourceIdentifier")]
            public string SourceIdentifier { get; set; }
        }
    }

    public class DDWorkNoticeSentResult
    {
        [JsonProperty("errcode")]
        public long ErrCode { get; set; }
        [JsonProperty("errmsg")]
        public string ErrMsg { get; set; }
        [JsonProperty("request_id")]
        public string RequestId { get; set; }
    }

    #region 发送钉钉通知模型
    public abstract class DDMessage
    {
        public string msgtype { get; protected set; }
    }

    public class Link
    {
        public string messageUrl { get; set; }
        public string picUrl { get; set; }
        public string title { get; set; }
        public string text { get; set; }
    }

    public class Text
    {
        public string content { get; set; }
    }

    public sealed class DDLinkMessage : DDMessage
    {
        public DDLinkMessage()
        {
            msgtype = "link";
        }

        public Link link { get; set; }
    }

    public sealed class DDTextMessage : DDMessage
    {
        public DDTextMessage()
        {
            msgtype = "text";
        }

        public Text text { get; set; }
    }

    #endregion


}