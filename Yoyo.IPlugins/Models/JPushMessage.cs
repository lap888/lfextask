using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Models
{
    /// <summary>
    /// 自定义消息
    /// </summary>
    public class JPushMessage
    {
        /// <summary>
        /// 消息内容本身（必填）。
        /// </summary>
        [JsonProperty("msg_content")]
        public string Content { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        [JsonProperty("extras")]
        public IDictionary Extras { get; set; }
    }
}
