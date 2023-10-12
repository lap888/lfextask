using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Yoyo.IPlugins.Utils
{
    /// <summary>
    /// 实名认证
    /// </summary>
    public class RealVerifyResponse
    {
        /// <summary>
        /// 响应报文
        /// </summary>
        public String Content { get; set; }

        public Boolean IsError
        {
            get
            {
                if (this.ErrCode == "200")
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 请求码
        /// </summary>
        [JsonProperty("requestId")]
        public String RequestId { get; set; }

        /// <summary>
        /// 错误码
        /// </summary>
        [JsonProperty("code")]
        public String ErrCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [JsonProperty("message")]
        public String ErrMsg { get; set; }

    }
}
