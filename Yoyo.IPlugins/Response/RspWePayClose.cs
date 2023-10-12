using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Yoyo.IPlugins.Response
{
    /// <summary>
    /// 关闭订单
    /// </summary>
    [Serializable]
    [XmlRoot("xml")]
    public class RspWePayClose : Utils.WePayResponse
    {
        /// <summary>
        /// 小程序ID
        /// </summary>
        [XmlElement("appid")]
        public String AppId { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [XmlElement("mch_id")]
        public String MchId { get; set; }

        /// <summary>
        /// 随机字符串
        /// </summary>
        [XmlElement("nonce_str")]
        public String NonceStr { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        [XmlElement("sign")]
        public String Sign { get; set; }
    }
}
