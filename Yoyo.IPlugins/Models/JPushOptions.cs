using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Models
{
    public class JPushOptions
    {
        /// <summary>
        /// 推送序号。
        /// 用来作为 API 调用标识，API 返回时被原样返回，以方便 API 调用方匹配请求与返回。
        /// 不能为 0。
        /// </summary>
        [JsonProperty("sendno", NullValueHandling = NullValueHandling.Ignore)]
        public int? SendNo { get; set; }

        /// <summary>
        /// 离线消息保留时长(秒)。
        /// 推送当前用户不在线时，为该用户保留多长时间的离线消息，以便其上线时再次推送。
        /// 默认 86400 （1 天），最长 10 天。
        /// 设置为 0 表示不保留离线消息，只有推送当前在线的用户可以收到。
        /// </summary>
        [JsonProperty("time_to_live", NullValueHandling = NullValueHandling.Ignore)]
        public int? TimeToLive { get; set; }

        /// <summary>
        /// 要覆盖的消息 ID。
        /// 如果当前的推送要覆盖之前的一条推送，这里填写前一条推送的 msg_id 就会产生覆盖效果。
        /// 覆盖功能起作用的时限是：1 天。
        /// </summary>
        [JsonProperty("override_msg_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? OverrideMessageId { get; set; }

        /// <summary>
        /// iOS 推送是否为生产环境。默认为 false - 开发环境。
        /// true: 生产环境；false: 开发环境。
        /// </summary>
        [JsonProperty("apns_production", DefaultValueHandling = DefaultValueHandling.Include)]
        public bool IsApnsProduction { get; set; } = false;

        /// <summary>
        /// 更新 iOS 通知的标识符。
        /// APNs 新通知如果匹配到当前通知中心有相同 apns-collapse-id 字段的通知，
        /// 则会用新通知内容来更新它，并使其置于通知中心首位。collapse id 长度不可超过 64 bytes。
        /// </summary>
        [JsonProperty("apns_collapse_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ApnsCollapseId { get; set; }

        /// <summary>
        /// 定速推送时长（分钟）。
        /// 又名缓慢推送。把原本尽可能快的推送速度，降低下来，给定的 n 分钟内，
        /// 均匀地向这次推送的目标用户推送。最大值为 1400，未设置则不是定速推送。
        /// </summary>
        [JsonProperty("big_push_duration", NullValueHandling = NullValueHandling.Ignore)]
        public int? BigPushDuration { get; set; }

        /// <summary>
        /// 自定义参数
        /// </summary>
        public Dictionary<string, object> Dict { get; set; }

        public void Add(string key, object value)
        {
            if (Dict == null)
            {
                Dict = new Dictionary<string, object>();
            }
            Dict.Add(key, value);
        }
    }
}
