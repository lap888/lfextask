using System;
using System.Collections.Generic;
using System.Linq;

namespace Yoyo.Core.Expand
{
    /// <summary>
    /// 对象转换拓展
    /// </summary>
    public static class ObjectChangeExpand
    {
        #region 数字转中文
        public static string ToChinese(this int number)
        {
            string res = string.Empty;
            string str = number.ToString();
            string schar = str.Substring(0, 1);
            switch (schar)
            {
                case "1":
                    res = "一";
                    break;
                case "2":
                    res = "二";
                    break;
                case "3":
                    res = "三";
                    break;
                case "4":
                    res = "四";
                    break;
                case "5":
                    res = "五";
                    break;
                case "6":
                    res = "六";
                    break;
                case "7":
                    res = "七";
                    break;
                case "8":
                    res = "八";
                    break;
                case "9":
                    res = "九";
                    break;
                default:
                    res = "零";
                    break;
            }
            if (str.Length > 1)
            {
                switch (str.Length)
                {
                    case 2:
                    case 6:
                        res += "十";
                        break;
                    case 3:
                    case 7:
                        res += "百";
                        break;
                    case 4:
                        res += "千";
                        break;
                    case 5:
                        res += "万";
                        break;
                    default:
                        res += "";
                        break;
                }
                res += ToChinese(int.Parse(str.Substring(1, str.Length - 1)));
            }
            return res;
        }
        #endregion

        #region 转Int
        /// <summary>
        /// 将object类型转换成int类型
        /// </summary>
        /// <param name="o">目标对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static int ToInt(this object o, int defaultValue)
        {
            if (o == null) { return defaultValue; }
            var s = o.ToString();
            if (!string.IsNullOrWhiteSpace(s))
            {
                if (int.TryParse(s, out int result))
                {
                    return result;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// 将object类型转换成int类型
        /// </summary>
        /// <param name="o">目标对象</param>
        /// <returns></returns>
        public static int ToInt(this object o)
        {
            return ToInt(o, 0);
        }
        #endregion

        #region 转Long
        /// <summary>
        /// 将object类型转换成int类型
        /// </summary>
        /// <param name="o">目标对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static long ToLong(this object o, long defaultValue)
        {
            if (o == null) { return defaultValue; }
            var s = o.ToString();
            if (!string.IsNullOrWhiteSpace(s))
            {
                if (long.TryParse(s, out long result))
                {
                    return result;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// 将object类型转换成int类型
        /// </summary>
        /// <param name="o">目标对象</param>
        /// <returns></returns>
        public static long ToLong(this object o)
        {
            return ToLong(o, 0);
        }

        #endregion

        #region 转Bool
        /// <summary>
        /// 将object类型转换成bool类型
        /// </summary>
        /// <param name="o">目标对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static bool ToBool(this object o, bool defaultValue)
        {
            if (o == null) { return defaultValue; }
            var s = o.ToString();
            if (s == "false")
            {
                return false;
            }
            if (s == "true")
            {
                return true;
            }
            return defaultValue;
        }

        /// <summary>
        /// 将object类型转换成bool类型
        /// </summary>
        /// <param name="o">目标对象</param>
        /// <returns></returns>
        public static bool ToBool(this object o)
        {
            return ToBool(o, false);
        }

        #endregion

        #region 转DateTime
        /// <summary>
        /// 将object类型转换成datetime类型
        /// </summary>
        /// <param name="o">目标对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object o, DateTime defaultValue)
        {
            if (o == null) { return defaultValue; }
            var s = o.ToString();
            if (!string.IsNullOrWhiteSpace(s))
            {
                DateTime result;
                if (DateTime.TryParse(s, out result)) { return result; }
            }
            return defaultValue;
        }

        /// <summary>
        /// 将object类型转换成datetime类型
        /// </summary>
        /// <param name="o">目标对象</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object o)
        {
            return ToDateTime(o, DateTime.Now);
        }

        #endregion

        #region 时间转换微时间戳
        /// <summary>
        /// datetime转换为unixtime
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToUnixTime(this DateTime value)
        {
            DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            int timeStamp = Convert.ToInt32((value - dateStart).TotalSeconds);
            return timeStamp;
        }
        #endregion

        #region 时间戳转换为时间
        /// <summary>
        /// unix时间转换为datetime
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime UnixToDateTime(this int value)
        {
            return UnixToDateTime((long)value, false);
        }
        /// <summary>
        /// unix时间转换为datetime
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isMiliSecond"></param>
        /// <returns></returns>
        public static DateTime UnixToDateTime(this long value, bool isMiliSecond = true)
        {
            long mili = 10000000;
            if (isMiliSecond) { mili = mili / 1000; }
            DateTime dtStart = new DateTime(1970, 1, 1, 8, 0, 0);
            long lTime = ((long)value * mili);
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime targetDt = dtStart.Add(toNow);
            return targetDt;
        }
        #endregion

        #region 转Decimal
        /// <summary>
        /// 将object类型转换成decimal类型
        /// </summary>
        /// <param name="o">目标对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static decimal ToDecimal(this object o, decimal defaultValue = 0M)
        {
            if (o == null) { return defaultValue; }
            var s = o.ToString();
            if (!string.IsNullOrWhiteSpace(s))
            {
                if (decimal.TryParse(s, out decimal result)) { return result; }
            }

            return defaultValue;
        }

        /// <summary>
        /// 将object类型转换成decimal类型
        /// </summary>
        /// <param name="o">目标对象</param>
        /// <returns></returns>
        public static decimal ToDecimal(this object o)
        {
            return ToDecimal(o, 0M);
        }

        #endregion

        #region 泛型转换
        /// <summary>
        /// 转换为排序键值对
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="model">模型</param>
        /// <returns>键值对</returns>
        public static SortedDictionary<String, Object> ToSortedDictionary<T>(this T model) where T : class
        {
            if (null == model) { return new SortedDictionary<string, object>(); }
            return model.ToJson().JsonTo<SortedDictionary<string, object>>();
        }
        /// <summary>
        /// 转换为键值对
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="model">模型</param>
        /// <returns>键值对</returns>
        public static Dictionary<String, Object> ToDictionary<T>(this T model) where T : class
        {
            if (null == model) { return new Dictionary<string, object>(); }
            return model.ToJson().JsonTo<Dictionary<string, object>>();
        }
        /// <summary>
        /// 排序键值对转换为模型
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public static T ToModel<T>(this SortedDictionary<String, Object> keyValues) where T : class, new()
        {
            if (null == keyValues) { return default(T); }
            return keyValues.ToJson().JsonTo<T>();
        }
        /// <summary>
        /// 键值对转换为模型
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public static T ToModel<T>(this Dictionary<String, Object> keyValues) where T : class, new()
        {
            if (null == keyValues) { return default(T); }
            return keyValues.ToJson().JsonTo<T>();
        }
        #endregion

        #region 任意进制转换
        /// <summary>
        /// 任意进制转换
        /// </summary>
        /// <param name="sourceValue">数据源</param>
        /// <param name="sourceBaseChars">源进制，默认16进制</param>
        /// <param name="newBaseChars">目标进制，默认10进制</param>
        /// <returns></returns>
        public static string BaseConvert(string sourceValue, string sourceBaseChars = "0123456789ABCDEF", string newBaseChars = "0123456789")
        {
            //M进制
            var sBase = sourceBaseChars.Length;
            //N进制
            var tBase = newBaseChars.Length;
            //M进制数字字符串合法性判断（判断M进制数字字符串中是否有不包含在M进制字符集中的字符）
            if (sourceValue.Any(s => !sourceBaseChars.Contains(s))) return null;

            //将M进制数字字符串的每一位字符转为十进制数字依次存入到LIST中
            var intSource = new List<int>();
            intSource.AddRange(sourceValue.Select(c => sourceBaseChars.IndexOf(c)));

            //余数列表
            var res = new List<int>();

            //开始转换（判断十进制LIST是否为空或只剩一位且这个数字小于N进制）
            while (!((intSource.Count == 1 && intSource[0] < tBase) || intSource.Count == 0))
            {

                //每一轮的商值集合
                var ans = new List<int>();

                var y = 0;
                //十进制LIST中的数字逐一除以N进制（注意：需要加上上一位计算后的余数乘以M进制）
                foreach (var t in intSource)
                {
                    //当前位的数值加上上一位计算后的余数乘以M进制
                    y = y * sBase + t;
                    //保存当前位与N进制的商值
                    ans.Add(y / tBase);
                    //计算余值
                    y %= tBase;
                }
                //将此轮的余数添加到余数列表
                res.Add(y);

                //将此轮的商值（去除0开头的数字）存入十进制LIST做为下一轮的被除数
                var flag = false;
                intSource.Clear();
                foreach (var a in ans.Where(a => a != 0 || flag))
                {
                    flag = true;
                    intSource.Add(a);
                }
            }
            //如果十进制LIST还有数字，需将此数字添加到余数列表后
            if (intSource.Count > 0) res.Add(intSource[0]);

            //将余数列表反转，并逐位转换为N进制字符
            var nValue = string.Empty;
            for (var i = res.Count - 1; i >= 0; i--)
            {
                nValue += newBaseChars[res[i]].ToString();
            }

            return nValue;
        }
        #endregion

    }
}
