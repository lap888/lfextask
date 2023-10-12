using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Yoyo.IPlugins.Utils;

namespace Yoyo.IPlugins.Response
{
    /// <summary>
    /// 查询退款
    /// </summary>
    public class RspAlipayRefundQuery : AlipayResponse
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
        /// 本笔退款对应的退款请求号
        /// </summary>
        [JsonProperty("out_request_no")]
        public String OutRequestNo { get; set; }

        /// <summary>
        /// 发起退款时，传入的退款原因
        /// </summary>
        [JsonProperty("refund_reason")]
        public String RefundReason { get; set; }

        /// <summary>
        /// 该笔退款所对应的交易的订单金额
        /// </summary>
        [JsonProperty("total_amount")]
        public Decimal? TotalAmount { get; set; }

        /// <summary>
        /// 本次退款请求，对应的退款金额
        /// </summary>
        [JsonProperty("refund_amount")]
        public Decimal? RefundAmount { get; set; }

        /// <summary>
        /// 退款支付时间
        /// </summary>
        [JsonProperty("gmt_refund_pay")]
        public DateTime? GmtRefundPay { get; set; }

        /// <summary>
        /// 退分账明细信息
        /// </summary>
        [JsonProperty("refund_royaltys")]
        public List<UtilDictionary> RefundRoyaltys { get; set; }

        /// <summary>
        /// 本次退款使用的资金渠道；
        /// 默认不返回该信息，需与支付宝约定后配置返回；
        /// </summary>
        [JsonProperty("refund_detail_item_list")]
        public List<UtilDictionary> RefundDetailItemList { get; set; }

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

    }
}
