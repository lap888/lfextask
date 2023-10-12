using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Response
{
    /// <summary>
    /// 手机网站支付
    /// </summary>
    public class RspAlipayWapSubmit : Utils.AlipayResponse
    {
        /// <summary>
        /// 支付宝交易号
        /// </summary>
        [JsonProperty("trade_no")]
        public String TradeNo { get; set; }

        /// <summary>
        /// 商家订单号
        /// </summary>
        [JsonProperty("out_trade_no")]
        public String OutTradeNo { get; set; }

        /// <summary>
        /// 该笔订单的资金总额，
        /// 单位为RMB-Yuan。
        /// </summary>
        [JsonProperty("total_amount")]
        public String TotalAmount { get; set; }

        /// <summary>
        /// 收款支付宝账号对应的支付宝唯一用户号。
        /// 以2088开头的纯16位数字
        /// </summary>
        [JsonProperty("seller_id")]
        public String SellerId { get; set; }

        /// <summary>
        /// 商户原始订单号，最大长度限制32位
        /// </summary>
        [JsonProperty("merchant_order_no")]
        public String MerchantOrderNo { get; set; }
    }
}
