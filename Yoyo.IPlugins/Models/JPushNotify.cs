using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Models
{
    /// <summary>
    /// 通知
    /// </summary>
    public class JPushNotify
    {
        [JsonProperty("alert")]
        public string Alert { get; set; }

        [JsonProperty("android", NullValueHandling = NullValueHandling.Ignore)]
        public JPushAndroid Android { get; set; }

        [JsonProperty("ios", NullValueHandling = NullValueHandling.Ignore)]
        public JPushIOS IOS { get; set; }
    }
}
