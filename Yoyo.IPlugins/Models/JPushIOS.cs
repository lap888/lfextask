using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Models
{
    /// <summary>
    /// 苹果
    /// </summary>
    public class JPushIOS
    {
        /// <summary>
        /// 可以是 string，也可以是 Apple 官方定义的 alert payload 结构。
        /// https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/PayloadKeyReference.html#//apple_ref/doc/uid/TP40008194-CH17-SW5
        /// </summary>
        [JsonProperty("alert")]
        public object Alert { get; set; }

        [JsonProperty("sound", NullValueHandling = NullValueHandling.Ignore)]
        public string Sound { get; set; }

        /// <summary>
        /// 默认角标 +1。
        /// </summary>
        [JsonProperty("badge")]
        public string Badge { get; set; } = "+1";

        [JsonProperty("content-available", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ContentAvailable { get; set; }

        [JsonProperty("mutable-content", NullValueHandling = NullValueHandling.Ignore)]
        public bool? MutableContent { get; set; }

        [JsonProperty("category", NullValueHandling = NullValueHandling.Ignore)]
        public string Category { get; set; }

        [JsonProperty("extras", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Extras { get; set; }

        [JsonProperty("thread-id", NullValueHandling = NullValueHandling.Ignore)]
        public string ThreadId { get; set; }
    }
}
