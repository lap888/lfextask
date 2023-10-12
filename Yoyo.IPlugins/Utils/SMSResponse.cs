using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Yoyo.IPlugins.Utils
{
    /// <summary>
    /// 统一响应模型
    /// </summary>
    public class SMSResponse
    {
        /// <summary>
        /// 响应报文
        /// </summary>
        public String Content { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [JsonProperty]
        public ErrorBody Error { get; set; }

        /// <summary>
        /// 是否错误
        /// </summary>
        public Boolean IsError
        {
            get
            {
                if (this.Error != null)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 错误码
        /// </summary>
        public String ErrCode
        {
            get
            {
                return this.Error?.ErrCode;
            }
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public String ErrMsg
        {
            get
            {
                return this.Error?.ErrMsg;
            }
        }
    }

    /// <summary>
    /// 错误模型
    /// </summary>
    public class ErrorBody
    {
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
