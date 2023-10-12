using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Yoyo.IPlugins.Response
{
    /// <summary>
    /// 单笔转账接口
    /// </summary>
    public class RspAlipayTransfer : Utils.AlipayResponse
    {
        /// <summary>
        /// 商户订单号
        /// </summary>
        [JsonProperty("out_biz_no")]
        public String OutBizNo { get; set; }

        /// <summary>
        /// 支付宝转账订单号
        /// </summary>
        [JsonProperty("order_id")]
        public String OrderId { get; set; }

        /// <summary>
        /// 支付宝支付资金流水号
        /// </summary>
        [JsonProperty("pay_fund_order_id")]
        public String PayFundOrderId { get; set; }

        /// <summary>
        /// 转账单据状态。
        /// SUCCESS：成功（对转账到银行卡的单据, 该状态可能变为退票[REFUND] 状态）；
        /// FAIL：失败（具体失败原因请参见error_code以及fail_reason返回值）；
        /// DEALING：处理中；
        /// REFUND：退票；
        /// </summary>
        [JsonProperty("status")]
        public String Status { get; set; }

        /// <summary>
        /// 订单支付时间，格式为yyyy-MM-dd HH:mm:ss
        /// </summary>
        [JsonProperty("trans_date")]
        public DateTime? TransDate { get; set; }
    }
}
