using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Yoyo.IPlugins.Models;
using Yoyo.IPlugins.Utils;

namespace Yoyo.IPlugins.Request
{
    /// <summary>
    /// 推送
    /// </summary>
    public class ReqJPushSend
    {
        /// <summary>
        /// 推送唯一标识符
        /// </summary>
        [JsonProperty("cid")]
        public String CId { get; set; }

        /// <summary>
        /// 推送平台。
        /// 可以为 "android" / "ios" / "all"。
        /// </summary>
        [JsonProperty("platform", DefaultValueHandling = DefaultValueHandling.Include)]
        public Enums.JPushPlatform Platform { get; set; } = Enums.JPushPlatform.all;

        /// <summary>
        /// 推送目标
        /// </summary>
        [JsonProperty("audience", DefaultValueHandling = DefaultValueHandling.Include)]
        public Object Audience { get; set; } = "all";

        /// <summary>
        /// 通知
        /// </summary>
        [JsonProperty("notification", NullValueHandling = NullValueHandling.Ignore)]
        public JPushNotify Notification { get; set; }

        /// <summary>
        /// 自定义消息
        /// </summary>
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public JPushMessage Message { get; set; }

        /// <summary>
        /// 短信补充
        /// </summary>
        [JsonProperty("sms_message", NullValueHandling = NullValueHandling.Ignore)]
        public JPushSmsMessage SMSMessage { get; set; }

        /// <summary>
        /// 可选参数
        /// </summary>
        [JsonProperty("options", DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof(JPushOptionsConvert))]
        public JPushOptions Options { get; set; } = new JPushOptions
        {
            IsApnsProduction = false
        };
    }
}
