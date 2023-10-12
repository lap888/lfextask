using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Utils
{
    public class JPushOptionsConvert : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new Exception("Unsupport ReadJson convert.");
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType.FullName == typeof(Models.JPushOptions).FullName)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteStartObject();
            Models.JPushOptions options = (Models.JPushOptions)value;
            if (options.SendNo != null)
            {
                writer.WritePropertyName("sendno");
                writer.WriteValue(options.SendNo);
            }
            if (options.TimeToLive != null)
            {
                writer.WritePropertyName("time_to_live");
                writer.WriteValue(options.TimeToLive);
            }
            if (options.OverrideMessageId != null)
            {
                writer.WritePropertyName("override_msg_id");
                writer.WriteValue(options.OverrideMessageId);
            }
            writer.WritePropertyName("apns_production");
            writer.WriteValue(options.IsApnsProduction);
            if (options.ApnsCollapseId != null)
            {
                writer.WritePropertyName("apns_collapse_id");
                writer.WriteValue(options.ApnsCollapseId);
            }
            if (options.BigPushDuration != null)
            {
                writer.WritePropertyName("big_push_duration");
                writer.WriteValue(options.BigPushDuration);
            }
            if (options.Dict != null)
            {
                foreach (KeyValuePair<string, object> item in options.Dict)
                {
                    writer.WritePropertyName(item.Key);
                    serializer.Serialize(writer, item.Value);
                }
            }
        }
    }
}
