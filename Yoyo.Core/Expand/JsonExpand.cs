using System;

namespace Yoyo.Core.Expand
{
    /// <summary>
    /// JSON拓展方法
    /// </summary>
    public static class JsonExpand
    {
        #region Json转换方法
        /// <summary>
        /// 转换对象为Json
        /// </summary>
        /// <param name="Obj">需要转换的对象</param>
        /// <param name="WithFormat">是否包含字符格式</param>
        /// <param name="WithNull">是否包含Null值</param>
        /// <returns></returns>
        public static string ToJson(this object Obj, bool WithFormat = true, bool WithNull = true)
        {
            try
            {
                if (null == Obj) { return String.Empty; }
                Newtonsoft.Json.Formatting JFormat = Newtonsoft.Json.Formatting.None;
                Newtonsoft.Json.NullValueHandling JNull = Newtonsoft.Json.NullValueHandling.Ignore;
                if (WithFormat) { JFormat = Newtonsoft.Json.Formatting.Indented; }
                if (WithNull) { JNull = Newtonsoft.Json.NullValueHandling.Include; }
                Newtonsoft.Json.JsonSerializerSettings JSet = new Newtonsoft.Json.JsonSerializerSettings { NullValueHandling = JNull, DateFormatString = "yyyy-MM-dd HH:mm:ss" };
                return Newtonsoft.Json.JsonConvert.SerializeObject(Obj, JFormat, JSet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// Json字符串转换为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="JsonStr">Json字符串</param>
        /// <returns></returns>
        public static T JsonTo<T>(this string JsonStr) where T : class, new()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(JsonStr)) { throw new ArgumentException("The parameter is empty"); }
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(JsonStr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
