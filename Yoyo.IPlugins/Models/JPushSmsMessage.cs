using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Models
{
    /// <summary>
    /// 短信补充
    /// </summary>
    public class JPushSmsMessage
    {
        [JsonProperty("delay_time", DefaultValueHandling = DefaultValueHandling.Include)]
        public int DelayTime { get; set; }

        [JsonProperty("signid", NullValueHandling = NullValueHandling.Ignore)]
        public int Signid { get; set; }

        [JsonProperty("temp_id", DefaultValueHandling = DefaultValueHandling.Include)]
        public long TempId { get; set; }

        [JsonProperty("temp_para", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> TempPara { get; set; }

        [JsonProperty("active_filter", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ActiveFilter { get; set; }
    }
}
