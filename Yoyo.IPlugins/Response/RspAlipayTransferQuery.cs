using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Yoyo.IPlugins.Utils;

namespace Yoyo.IPlugins.Response
{
    /// <summary>
    /// 转账业务单据查询接口
    /// </summary>
    public class RspAlipayTransferQuery : Utils.AlipayResponse
    {
        /// <summary>
        /// 支付宝转账单据号，查询失败不返回。
        /// </summary>
        [JsonProperty("order_id")]
        public String OrderId { get; set; }

        /// <summary>
        /// 支付宝支付资金流水号，转账失败不返回。
        /// </summary>
        [JsonProperty("pay_fund_order_id")]
        public String PayFundOrderId { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        [JsonProperty("out_biz_no")]
        public String OutBizNo { get; set; }

        /// <summary>
        /// 付款金额，收银台场景下付款成功后的支付金额，订单状态为SUCCESS才返回，其他状态不返回。
        /// </summary>
        [JsonProperty("trans_amount")]
        public Decimal TransAmount { get; set; }

        /// <summary>
        /// 转账单据状态。可能出现的状态如下：
        /// SUCCESS：转账成功；
        /// WAIT_PAY：等待支付；
        /// CLOSED：订单超时关闭
        /// alipay.fund.trans.app.pay涉及的状态： WAIT_PAY、SUCCESS、CLOSED
        /// alipay.fund.trans.uni.transfer、alipay.fund.trans.refund涉及的状态：SUCCESS
        /// </summary>
        [JsonProperty("status")]
        public String Status { get; set; }

        /// <summary>
        /// 支付时间，格式为yyyy-MM-dd HH:mm:ss，转账失败不返回。
        /// </summary>
        [JsonProperty("pay_date")]
        public DateTime? PayDate { get; set; }

        /// <summary>
        /// 预计到账时间，转账到银行卡专用，格式为yyyy-MM-dd HH:mm:ss，转账受理失败不返回。
        /// 注意：此参数为预计时间，可能与实际到账时间有较大误差，不能作为实际到账时间使用，仅供参考用途。
        /// </summary>
        [JsonProperty("arrival_time_end")]
        public String ArrivalTimeEnd { get; set; }

        /// <summary>
        /// 预计收费金额（元），转账到银行卡专用，数字格式，精确到小数点后2位，
        /// 转账失败或转账受理失败不返回。
        /// </summary>
        [JsonProperty("order_fee")]
        public String OrderFee { get; set; }

        /// <summary>
        /// 查询到的订单状态为FAIL失败或REFUND退票时，返回错误代码
        /// </summary>
        [JsonProperty("error_code")]
        public String ErrorCode { get; set; }

        /// <summary>
        /// 查询到的订单状态为FAIL失败或REFUND退票时，返回具体的原因。
        /// </summary>
        [JsonProperty("fail_reason")]
        public String FailReason { get; set; }

        /// <summary>
        /// 商户查询代扣订单信息时返回其在代扣请求中传入的账单属性
        /// </summary>
        [JsonProperty("deduct_bill_info")]
        public UtilDictionary DeductBillInfo { get; set; }

        /// <summary>
        /// 商户在查询代发订单信息时返回其在代发请求中传入的账单属性。
        /// </summary>
        [JsonProperty("transfer_bill_info")]
        public UtilDictionary TransferBillInfo { get; set; }
    }
}
