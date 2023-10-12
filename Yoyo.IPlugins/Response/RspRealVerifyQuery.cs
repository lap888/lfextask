using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Response
{
    /// <summary>
    /// 查询认证结果
    /// </summary>
    public class RspRealVerifyQuery : Utils.RealVerifyResponse
    {
        /// <summary>
        /// 响应内容
        /// </summary>
        [JsonProperty("data")]
        public RealVerifyResult Data { get; set; }
    }

    /// <summary>
    /// 认证结果
    /// </summary>
    public class RealVerifyResult
    {
        /// <summary>
        /// 是否通过，通过为T，不通过为F。
        /// </summary>
        [JsonProperty("passed")]
        public String Passed { get; set; }

        /// <summary>
        /// 是否通过
        /// </summary>
        [JsonIgnore]
        public Boolean IsPass
        {
            get
            {
                return this.Passed.Equals("T");
            }
        }

        /// <summary>
        /// 认证的主体信息，一般的认证场景返回为空。
        /// </summary>
        [JsonProperty("identityInfo")]
        public String IdentityInfo { get; set; }

        /// <summary>
        /// 认证主体附件信息，主要为图片类材料，一般的认证场景都是返回空。
        /// </summary>
        [JsonProperty("materialInfo")]
        public String MaterialInfo { get; set; }
    }
}
