using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Response
{
    /// <summary>
    /// 关闭订单
    /// </summary>
    public class RspAlipayTradeClose : Utils.AlipayResponse
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
    }
}
