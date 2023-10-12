using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Utils
{
    /// <summary>
    /// 键值对
    /// </summary>
    public class UtilDictionary : Dictionary<String, String>
    {
        public UtilDictionary() { }

        public UtilDictionary(IDictionary<String, String> dictionary) : base(dictionary) { }


        /// <summary>
        /// 添加一个新的键值对。空键或者空值的键值对将会被忽略。
        /// </summary>
        /// <param name="key">键名称</param>
        /// <param name="value">键对应的值，目前支持：string, int, long, double, bool, DateTime类型</param>
        public void Add(String key, Object value)
        {
            String strValue;

            if (value == null)
            {
                strValue = null;
            }
            else if (value is String)
            {
                strValue = (String)value;
            }
            else if (value is Nullable<DateTime>)
            {
                Nullable<DateTime> dateTime = value as Nullable<DateTime>;
                strValue = dateTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else if (value is Nullable<Int32>)
            {
                strValue = (value as Nullable<Int32>).Value.ToString();
            }
            else if (value is Nullable<Int64>)
            {
                strValue = (value as Nullable<Int64>).Value.ToString();
            }
            else if (value is Nullable<Double>)
            {
                strValue = (value as Nullable<Double>).Value.ToString();
            }
            else if (value is Nullable<Boolean>)
            {
                strValue = (value as Nullable<Boolean>).Value.ToString().ToLower();
            }
            else if (value is UtilDictionary || value is List<UtilDictionary>)
            {
                strValue = JsonConvert.SerializeObject((value as UtilDictionary));
            }
            else if (value is List<String>)
            {
                strValue = JsonConvert.SerializeObject((value as List<String>));
            }
            else
            {
                strValue = value.ToString();
            }

            this.Add(key, strValue);
        }

        public new void Add(String key, String value)
        {
            if (!String.IsNullOrEmpty(key) && !String.IsNullOrEmpty(value))
            {
                base[key] = value;
            }
        }

        public void AddAll(IDictionary<String, String> dict)
        {
            if (dict != null && dict.Count > 0)
            {
                IEnumerator<KeyValuePair<String, String>> kvps = dict.GetEnumerator();
                while (kvps.MoveNext())
                {
                    KeyValuePair<String, String> kvp = kvps.Current;
                    Add(kvp.Key, kvp.Value);
                }
            }
        }
    }
}
