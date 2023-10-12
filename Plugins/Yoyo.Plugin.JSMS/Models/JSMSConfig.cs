using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.Plugin.JSMS.Models
{
    /// <summary>
    /// 配置模型
    /// </summary>
    public class JSMSConfig
    {
        /// <summary>
        /// Client标识
        /// </summary>
        public String ClientName { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public String ApiUrl { get; set; }

        /// <summary>
        /// 应用标示
        /// </summary>
        public String AppKey { get; set; }

        /// <summary>
        /// 秘钥
        /// </summary>
        public String AppSecret { get; set; }
    }
}
