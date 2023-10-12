using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Yoyo.IPlugins.Request
{
    /// <summary>
    /// 发送魔板短信
    /// </summary>
    public class ReqSmsSend : Utils.ISMSRequest<Response.RspSmsSend>
    {
        public string GetUrl()
        {
            return "codes";
        }
        /// <summary>
        /// 手机号码
        /// </summary>
        [JsonProperty("mobile")]
        public String Mobile { get; set; }

        /// <summary>
        /// 签名ID
        /// </summary>
        [JsonProperty("sign_id")]
        public String SignId { get; set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        [JsonProperty("temp_id")]
        public String TempId { get; set; }

    }
}
