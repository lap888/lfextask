using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Yoyo.IPlugins.Response
{
    /// <summary>
    /// 发送短息
    /// </summary>
    public class RspSmsSend : Utils.SMSResponse
    {
        /// <summary>
        /// 消息编号
        /// </summary>
        [JsonProperty("msg_id")]
        public String MsgId { get; set; }
    }
}
