using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.Plugin.WeChatPay.Models
{
    /// <summary>
    /// 微信支付配置
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 支付请求
        /// </summary>
        public String ClientPay { get; set; }

        /// <summary>
        /// 退款请求
        /// </summary>
        public String ClientRefund { get; set; }

        /// <summary>
        /// 应用标示
        /// </summary>
        public String AppId { get; set; }

        /// <summary>
        /// 应用秘钥
        /// </summary>
        public String AppSecret { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public String MchId { get; set; }

        /// <summary>
        /// 秘钥
        /// </summary>
        public String ApiV3Key { get; set; }

        /// <summary>
        /// 证书路径
        /// </summary>
        public String CertPath { get; set; }

        /// <summary>
        /// 异步通知地址
        /// </summary>
        public String NotifyUrl { get; set; }

    }
}
