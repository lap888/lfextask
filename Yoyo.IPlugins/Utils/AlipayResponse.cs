using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Utils
{
    /// <summary>
    /// 支付宝支付BASE
    /// </summary>
    public class AlipayResponse
    {
        /// <summary>
        /// 网关返回码
        /// </summary>
        [JsonProperty("code")]
        public String Code { get; set; }

        /// <summary>
        /// 网关返回码描述
        /// </summary>
        [JsonProperty("msg")]
        public String Msg { get; set; }

        /// <summary>
        /// 业务返回码
        /// </summary>
        [JsonProperty("sub_code")]
        public String SubCode { get; set; }

        /// <summary>
        /// 业务返回码描述
        /// </summary>
        [JsonProperty("sub_msg")]
        public String SubMsg { get; set; }

    }
}
