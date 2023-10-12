using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Yoyo.IPlugins.Response
{
    /// <summary>
    /// 交易查询
    /// </summary>
    public class RspAlipayTradeQuery : Utils.AlipayResponse
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
        /// 交易的订单金额，单位为元，
        /// 两位小数。该参数的值为支付时传入的total_amount
        /// </summary>
        [JsonProperty("total_amount")]
        public Decimal TotalAmount { get; set; }

        /// <summary>
        /// 交易状态：WAIT_BUYER_PAY（交易创建，等待买家付款）、
        /// TRADE_CLOSED（未付款交易超时关闭，或支付完成后全额退款）、
        /// TRADE_SUCCESS（交易支付成功）、
        /// TRADE_FINISHED（交易结束，不可退款）
        /// </summary>
        [JsonProperty("trade_status")]
        public Enums.AlipayTradeStatus TradeStatus { get; set; }

        /// <summary>
        /// 标价币种，该参数的值为支付时传入的trans_currency，
        /// 支持英镑：GBP、港币：HKD、美元：USD、新加坡元：SGD、日元：JPY、加拿大元：CAD、澳元：AUD、欧元：EUR、新西兰元：NZD、韩元：KRW、
        /// 泰铢：THB、瑞士法郎：CHF、瑞典克朗：SEK、丹麦克朗：DKK、挪威克朗：NOK、马来西亚林吉特：MYR、印尼卢比：IDR、菲律宾比索：PHP、
        /// 毛里求斯卢比：MUR、以色列新谢克尔：ILS、斯里兰卡卢比：LKR、俄罗斯卢布：RUB、阿联酋迪拉姆：AED、捷克克朗：CZK、南非兰特：ZAR、
        /// 人民币：CNY、新台币：TWD。当trans_currency 和 settle_currency 不一致时，trans_currency支持人民币：CNY、新台币：TWD
        /// </summary>
        [JsonProperty("trans_currency")]
        public String TransCurrency { get; set; }

        /// <summary>
        /// 订单结算币种，对应支付接口传入的settle_currency，支持英镑：GBP、港币：HKD、美元：USD、新加坡元：SGD、日元：JPY、加拿大元：CAD、
        /// 澳元：AUD、欧元：EUR、新西兰元：NZD、韩元：KRW、泰铢：THB、瑞士法郎：CHF、瑞典克朗：SEK、丹麦克朗：DKK、挪威克朗：NOK、
        /// 马来西亚林吉特：MYR、印尼卢比：IDR、菲律宾比索：PHP、毛里求斯卢比：MUR、以色列新谢克尔：ILS、斯里兰卡卢比：LKR、俄罗斯卢布：RUB、
        /// 阿联酋迪拉姆：AED、捷克克朗：CZK、南非兰特：ZAR
        /// </summary>
        [JsonProperty("settle_currency")]
        public String SettleCurrency { get; set; }

        /// <summary>
        /// 结算币种订单金额
        /// </summary>
        [JsonProperty("settle_amount")]
        public Decimal? SettleAmount { get; set; }

        /// <summary>
        /// 订单支付币种
        /// </summary>
        [JsonProperty("pay_currency")]
        public String PayCurrency { get; set; }

        /// <summary>
        /// 支付币种订单金额
        /// </summary>
        [JsonProperty("pay_amount")]
        public Decimal? PayAmount { get; set; }

        /// <summary>
        /// 结算币种兑换标价币种汇率
        /// </summary>
        [JsonProperty("settle_trans_rate")]
        public String SettleTransRate { get; set; }

        /// <summary>
        /// 标价币种兑换支付币种汇率
        /// </summary>
        [JsonProperty("trans_pay_rate")]
        public String TransPayRate { get; set; }

        /// <summary>
        /// 买家实付金额，单位为元，两位小数。
        /// 该金额代表该笔交易买家实际支付的金额，不包含商户折扣等金额
        /// </summary>
        [JsonProperty("buyer_pay_amount")]
        public Decimal? BuyerPayAmount { get; set; }

        /// <summary>
        /// 积分支付的金额，单位为元，两位小数。
        /// 该金额代表该笔交易中用户使用积分支付的金额，比如集分宝或者支付宝实时优惠等
        /// </summary>
        [JsonProperty("point_amount")]
        public Decimal? PointAmount { get; set; }

        /// <summary>
        /// 交易中用户支付的可开具发票的金额，单位为元，两位小数。
        /// 该金额代表该笔交易中可以给用户开具发票的金额
        /// </summary>
        [JsonProperty("invoice_amount")]
        public Decimal? InvoiceAmount { get; set; }

        /// <summary>
        /// 本次交易打款给卖家的时间
        /// </summary>
        [JsonProperty("send_pay_date")]
        public DateTime? SendPayDate { get; set; }

        /// <summary>
        /// 实收金额，单位为元，两位小数。
        /// 该金额为本笔交易，商户账户能够实际收到的金额
        /// </summary>
        [JsonProperty("receipt_amount")]
        public String ReceiptAmount { get; set; }

        /// <summary>
        /// 商户门店编号
        /// </summary>
        [JsonProperty("store_id")]
        public String StoreId { get; set; }

        /// <summary>
        /// 请求交易支付中的商户店铺的名称
        /// </summary>
        [JsonProperty("store_name")]
        public String StoreName { get; set; }

        /// <summary>
        /// 商户机具终端编号
        /// </summary>
        [JsonProperty("terminal_id")]
        public String TerminalId { get; set; }

        /// <summary>
        /// 交易支付使用的资金渠道
        /// </summary>
        [JsonProperty("fund_bill_list")]
        public TradeFundBill TradeFundBill { get; set; }

        /// <summary>
        /// 买家在支付宝的用户id
        /// </summary>
        [JsonProperty("buyer_user_id")]
        public String BuyerUserId { get; set; }

        /// <summary>
        /// 该笔交易针对收款方的收费金额；
        /// 默认不返回该信息，需与支付宝约定后配置返回；
        /// </summary>
        [JsonProperty("charge_amount")]
        public String ChargeAmount { get; set; }

        /// <summary>
        /// 费率活动标识，当交易享受活动优惠费率时，返回该活动的标识；
        /// 默认不返回该信息，需与支付宝约定后配置返回；
        /// 可能的返回值列表：蓝海活动标识：bluesea_1
        /// </summary>
        [JsonProperty("charge_flags")]
        public String ChargeFlags { get; set; }

        /// <summary>
        /// 支付清算编号，用于清算对账使用；
        /// 只在银行间联交易场景下返回该信息；
        /// </summary>
        [JsonProperty("settlement_id")]
        public String SettlementId { get; set; }

        /// <summary>
        /// 返回的交易结算信息，包含分账、补差等信息
        /// </summary>
        [JsonProperty("trade_settle_info")]
        public TradeSettleDetailList TradeSettleInfo { get; set; }

        /// <summary>
        /// 预授权支付模式，该参数仅在信用预授权支付场景下返回。
        /// 信用预授权支付：CREDIT_PREAUTH_PAY
        /// </summary>
        [JsonProperty("auth_trade_pay_mode")]
        public String AuthTradePayMode { get; set; }

        /// <summary>
        /// 买家用户类型。
        /// CORPORATE:企业用户；
        /// PRIVATE:个人用户。
        /// </summary>
        [JsonProperty("buyer_user_type")]
        public String BuyerUserType { get; set; }

        /// <summary>
        /// 商家优惠金额	
        /// </summary>
        [JsonProperty("mdiscount_amount")]
        public String MdiscountAmount { get; set; }

        /// <summary>
        /// 平台优惠金额
        /// </summary>
        [JsonProperty("discount_amount")]
        public String DiscountAmount { get; set; }

        /// <summary>
        /// 买家名称；
        /// 买家为个人用户时为买家姓名，买家为企业用户时为企业名称；
        /// 默认不返回该信息，需与支付宝约定后配置返回；
        /// </summary>
        [JsonProperty("buyer_user_name")]
        public String BuyerUserName { get; set; }

        /// <summary>
        /// 订单标题；
        /// 只在间连场景下返回；
        /// </summary>
        [JsonProperty("subject")]
        public String Subject { get; set; }

        /// <summary>
        /// 订单描述;
        /// 只在间连场景下返回；
        /// </summary>
        [JsonProperty("body")]
        public String Body { get; set; }

        /// <summary>
        /// 间连商户在支付宝端的商户编号；
        /// 只在间连场景下返回；
        /// </summary>
        [JsonProperty("alipay_sub_merchant_id")]
        public String AlipaySubMerchantId { get; set; }

        /// <summary>
        /// 交易额外信息，特殊场景下与支付宝约定返回。
        /// json格式。
        /// </summary>
        [JsonProperty("ext_infos")]
        public String ExtInfos { get; set; }
    }

    /// <summary>
    /// 交易支付使用的资金渠道
    /// </summary>
    public class TradeFundBill
    {
        /// <summary>
        /// 交易使用的资金渠道
        /// </summary>
        [JsonProperty("fund_channel")]
        public String FundChannel { get; set; }

        /// <summary>
        /// 银行卡支付时的银行代码
        /// </summary>
        [JsonProperty("bank_code")]
        public String BankCode { get; set; }

        /// <summary>
        /// 该支付工具类型所使用的金额	
        /// </summary>
        [JsonProperty("amount")]
        public Decimal? Amount { get; set; }

        /// <summary>
        /// 渠道实际付款金额
        /// </summary>
        [JsonProperty("real_amount")]
        public Decimal? RealAmount { get; set; }
    }

    /// <summary>
    /// 交易结算明细信息
    /// </summary>
    public class TradeSettleDetailList
    {
        public List<TradeSettleDetail> TradeSettlelList { get; set; }
    }

    /// <summary>
    /// 交易结算明细信息
    /// </summary>
    public class TradeSettleDetail
    {
        /// <summary>
        /// 结算操作类型。
        /// 包含replenish、replenish_refund、transfer、transfer_refund等类型
        /// </summary>
        [JsonProperty("operation_type")]
        public String OperationType { get; set; }

        /// <summary>
        /// 商户操作序列号。
        /// 商户发起请求的外部请求号。
        /// </summary>
        [JsonProperty("operation_serial_no")]
        public String OperationSerialNo { get; set; }

        /// <summary>
        /// 操作日期
        /// </summary>
        [JsonProperty("operation_dt")]
        public String OperationDt { get; set; }

        /// <summary>
        /// 转出账号
        /// </summary>
        [JsonProperty("trans_out")]
        public String TransOut { get; set; }

        /// <summary>
        /// 转入账号
        /// </summary>
        [JsonProperty("trans_in")]
        public String TransIn { get; set; }

        /// <summary>
        /// 实际操作金额，单位为元，两位小数。
        /// 该参数的值为分账或补差或结算时传入
        /// </summary>
        [JsonProperty("amount")]
        public Decimal? Amount { get; set; }
    }
}
