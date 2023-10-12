using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Yoyo.IPlugins.Utils
{
    /// <summary>
    /// 基础模型
    /// </summary>
    public class WePayResponse
    {
        /// <summary>
        /// 报文
        /// </summary>
        [XmlIgnore]
        public String Content { get; set; }

        /// <summary>
        /// 返回状态码
        /// </summary>
        [XmlElement("return_code")]
        public String ReturnCode { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        [XmlElement("return_msg")]
        public String ReturnMsg { get; set; }

        /// <summary>
        /// 业务结果
        /// </summary>
        [XmlElement("result_code")]
        public String ResultCode { get; set; }

        /// <summary>
        /// 业务结果描述
        /// </summary>
        [XmlElement("result_msg")]
        public String ResultMsg { get; set; }

        /// <summary>
        /// 错误代码
        /// </summary>
        [XmlElement("err_code")]
        public String ErrCode { get; set; }

        /// <summary>
        /// 错误代码描述
        /// </summary>
        [XmlElement("err_code_des")]
        public String ErrCodeDesc { get; set; }
    }
}
