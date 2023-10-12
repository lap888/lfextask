using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.Plugin.WeChat.Models
{
    /// <summary>
    /// 公众号配置
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 请求池
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// APPID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 凭证密钥
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// 获取ACCESS_TOKEN地址
        /// </summary>
        public string AccessTokenUrl { get; set; }
    }
}
