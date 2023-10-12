using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Models
{
    /// <summary>
    /// 安卓
    /// </summary>
    public class JPushAndroid
    {
        /// <summary>
        /// 必填。
        /// </summary>
        [JsonProperty("alert")]
        public string Alert { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("builder_id", NullValueHandling = NullValueHandling.Ignore)]
        public int? BuilderId { get; set; }

        [JsonProperty("channel_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ChannelId { get; set; }

        [JsonProperty("priority", NullValueHandling = NullValueHandling.Ignore)]
        public int? Priority { get; set; }

        [JsonProperty("category", NullValueHandling = NullValueHandling.Ignore)]
        public string Category { get; set; }

        [JsonProperty("style", NullValueHandling = NullValueHandling.Ignore)]
        public int? Style { get; set; }

        [JsonProperty("alert_type", NullValueHandling = NullValueHandling.Ignore)]
        public int? AlertType { get; set; }

        [JsonProperty("big_text", NullValueHandling = NullValueHandling.Ignore)]
        public string BigText { get; set; }

        [JsonProperty("inbox", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Inbox { get; set; }

        [JsonProperty("big_pic_path", NullValueHandling = NullValueHandling.Ignore)]
        public string BigPicturePath { get; set; }

        [JsonProperty("large_icon", NullValueHandling = NullValueHandling.Ignore)]
        public string LargeIcon { get; set; }

        [JsonProperty("intent", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Indent { get; set; }

        [JsonProperty("extras", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Extras { get; set; }

        /// <summary>
        /// (VIP only)指定开发者想要打开的 Activity，值为 <activity> 节点的 "android:name" 属性值。
        /// </summary>
        [JsonProperty("uri_activity", NullValueHandling = NullValueHandling.Ignore)]
        public string URIActivity { get; set; }

        /// <summary>
        /// (VIP only)指定打开 Activity 的方式，值为 Intent.java 中预定义的 "access flags" 的取值范围。
        /// </summary>
        [JsonProperty("uri_flag", NullValueHandling = NullValueHandling.Ignore)]
        public string URIFlag { get; set; }

        /// <summary>
        /// (VIP only)指定开发者想要打开的 Activity，值为 <activity> -> <intent-filter> -> <action> 节点中的 "android:name" 属性值。
        /// </summary>
        [JsonProperty("uri_action", NullValueHandling = NullValueHandling.Ignore)]
        public string URIAction { get; set; }
    }
}
