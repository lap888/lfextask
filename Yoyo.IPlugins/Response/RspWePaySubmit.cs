using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Yoyo.IPlugins.Response
{
    /// <summary>
    /// 下单
    /// </summary>
    [Serializable]
    [XmlRoot("xml")]
    public class RspWePaySubmit : Utils.WePayResponse
    {
        /// <summary>
        /// 小程序ID
        /// </summary>
        [XmlElement("appid")]
        public string AppId { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        [XmlElement("mch_id")]
        public string MchId { get; set; }

        /// <summary>
        /// 随机字符串
        /// </summary>
        [XmlElement("nonce_str")]
        public string NonceStr { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        [XmlElement("sign")]
        public string Sign { get; set; }

        /// <summary>
        /// 设备号
        /// </summary>
        [XmlElement("device_info")]
        public string DeviceInfo { get; set; }

    }
}
