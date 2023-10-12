using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Yoyo.IPlugins.Response
{
    /// <summary>
    /// 验证码验证
    /// </summary>
    public class RspSmsVerify : Utils.SMSResponse
    {
        /// <summary>
        /// 验证结果
        /// </summary>
        [JsonProperty("is_valid")]
        public Boolean IsValid { get; set; }
    }
}
