using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.Plugin.IQCloud.Models
{
    /// <summary>
    /// 腾讯云COS配置
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 请求标示
        /// </summary>
        public String ClientName { get; set; }
        /// <summary>
        /// 唯一资源标识
        /// </summary>
        public String AppId { get; set; }
        /// <summary>
        /// 项目身份识别 ID
        /// </summary>
        public String SecretId { get; set; }
        /// <summary>
        /// 项目身份密钥
        /// </summary>
        public String SecretKey { get; set; }
        /// <summary>
        /// 临时授权Id
        /// </summary>
        public String StsSecretId { get; set; }
        /// <summary>
        /// 临时授权秘钥
        /// </summary>
        public String StsSecretKey { get; set; }
        /// <summary>
        /// 存储数据的容器
        /// </summary>
        public String Bucket { get; set; }
        /// <summary>
        /// 地域信息
        /// </summary>
        public String Region { get; set; }
        /// <summary>
        /// 存储桶域名
        /// </summary>
        public String BucketDomain { get; set; }
    }
}
