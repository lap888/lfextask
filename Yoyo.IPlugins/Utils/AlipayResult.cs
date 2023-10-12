using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Yoyo.IPlugins.Utils
{
    /// <summary>
    /// 支付宝支付
    /// </summary>
    public class AlipayResult<T> where T : AlipayResponse, new()
    {
        /// <summary>
        /// 报文详情
        /// </summary>
        [JsonIgnore]
        public String Content { get; set; }

        /// <summary>
        /// 是否错误
        /// </summary>
        public Boolean IsError { get; set; } = true;

        /// <summary>
        /// 错误码
        /// </summary>
        [JsonIgnore]
        public String ErrCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [JsonIgnore]
        public String ErrMsg { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public T Result { get; set; }

        public String AlipayCertSN { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        [JsonProperty("sign")]
        public String Sign { get; set; }
    }
}
