using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Models
{
    /// <summary>
    /// 设备
    /// </summary>
    public class JDevicePayload
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("tags")]
        public Dictionary<string, object> Tags { get; set; }
    }
}
