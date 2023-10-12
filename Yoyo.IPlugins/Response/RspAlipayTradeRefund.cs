using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Response
{
    /// <summary>
    /// 交易退款
    /// </summary>
    public class RspAlipayTradeRefund : Utils.AlipayResponse
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
        /// 买家支付宝账号
        /// </summary>
        [JsonProperty("buyer_logon_id")]
        public String BuyerLogonId { get; set; }

        /// <summary>
        /// 本次退款是否发生了资金变化
        /// Y N
        /// </summary>
        [JsonProperty("fund_change")]
        public String FundChange { get; set; }

        /// <summary>
        /// 退款总金额
        /// </summary>
        [JsonProperty("refund_fee")]
        public Decimal? RefundFee { get; set; }

        /// <summary>
        /// 退款币种信息
        /// </summary>
        [JsonProperty("refund_currency")]
        public String RefundCurrency { get; set; }

        /// <summary>
        /// 退款支付时间
        /// </summary>
        [JsonProperty("gmt_refund_pay")]
        public DateTime? GmtRefundPay { get; set; }

        /// <summary>
        /// 交易在支付时候的门店名称
        /// </summary>
        [JsonProperty("store_name")]
        public String StoreName { get; set; }

        /// <summary>
        /// 买家在支付宝的用户id
        /// </summary>
        [JsonProperty("buyer_user_id")]
        public String BuyerUserId { get; set; }

        /// <summary>
        /// 退款清算编号，用于清算对账使用；
        /// 只在银行间联交易场景下返回该信息；
        /// </summary>
        [JsonProperty("refund_settlement_id")]
        public String RefundSettlementId { get; set; }

        /// <summary>
        /// 本次退款金额中买家退款金额
        /// </summary>
        [JsonProperty("present_refund_buyer_amount")]
        public String PresentRefundBuyerAmount { get; set; }

        /// <summary>
        /// 本次退款金额中平台优惠退款金额
        /// </summary>
        [JsonProperty("present_refund_discount_amount")]
        public String PresentRefundDiscountAmount { get; set; }

        /// <summary>
        /// 本次退款金额中商家优惠退款金额
        /// </summary>
        [JsonProperty("present_refund_mdiscount_amount")]
        public String PresentRefundMdiscountAmount { get; set; }

        /// <summary>
        /// 退款使用的资金渠道
        /// </summary>
        [JsonProperty("refund_detail_item_list")]
        public Object RefundDetailItemList { get; set; }

        /// <summary>
        /// 退回的前置资产列表
        /// </summary>
        [JsonProperty("refund_preset_paytool_list")]
        public Object RefundPresetPaytoolList { get; set; }
    }
}
