using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.Plugin.Alipay.Models
{
    /// <summary>
    /// 支付宝配置
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 请求标识
        /// </summary>
        public String ClientName { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public String ApiUrl { get; set; }

        /// <summary>
        /// 异步通知地址
        /// </summary>
        public String NotifyUrl { get; set; }

        /// <summary>
        /// 支付宝分配给开发者的应用ID
        /// </summary>
        public String AppId { get; set; }

        /// <summary>
        /// 应用私钥
        /// </summary>
        public String PrivateKey { get; set; }

        /// <summary>
        /// 支付宝公钥
        /// </summary>
        public String PublicKey { get; set; }

        /// <summary>
        /// 应用公钥证书SN
        /// </summary>
        public String AppCertSN { get; set; }

        /// <summary>
        /// 支付宝根证书SN
        /// </summary>
        public String AlipayCertSN { get; set; }
    }
}
