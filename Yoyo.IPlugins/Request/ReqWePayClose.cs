using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Request
{
    /// <summary>
    /// 关闭订单
    /// </summary>
    public class ReqWePayClose : Utils.IWePayRequest<Response.RspWePayClose>
    {
        /// <summary>
        /// 请求地址
        /// </summary>
        /// <returns></returns>
        public string GetUrl()
        {
            return "https://api.mch.weixin.qq.com/pay/closeorder";
        }

        /// <summary>
        /// 小程序ID
        /// </summary>
        public String AppId { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public String MchId { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public String TradeNo { get; set; }

        /// <summary>
        /// 随机字符串
        /// </summary>
        public String NonceStr { get; set; }

        /// <summary>
        /// 签名类型
        /// </summary>
        public String SignType { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        //public String Sign { get; set; }

        /// <summary>
        /// 获取请求参数
        /// </summary>
        /// <returns></returns>
        public Utils.WeXmlDoc GetXmlDoc()
        {
            Utils.WeXmlDoc XmlDoc = new Utils.WeXmlDoc();
            XmlDoc.Add("appid", this.AppId);
            XmlDoc.Add("mch_id", this.MchId);
            XmlDoc.Add("out_trade_no", this.TradeNo);
            XmlDoc.Add("nonce_str", this.NonceStr);
            XmlDoc.Add("sign_type", this.SignType);

            return XmlDoc;
        }
    }
}
