using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Yoyo.IPlugins.Request
{
    public class ReqSmsVerify : Utils.ISMSRequest<Response.RspSmsVerify>
    {
        public ReqSmsVerify(String MsgId)
        {
            this.MsgId = MsgId;
        }

        /// <summary>
        /// 消息编号
        /// </summary>
        [JsonIgnore]
        private String MsgId { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [JsonProperty("code")]
        public String Code { get; set; }

        public string GetUrl()
        {
            return $"codes/{this.MsgId}/valid";
        }
    }
}
