using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Yoyo.IPlugins.Response
{
    /// <summary>
    /// 订单查询
    /// </summary>
    [Serializable]
    [XmlRoot("xml")]
    public class RspWePayQuery : Utils.WePayResponse
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

        /// <summary>
        /// 设备号
        /// </summary>
        [XmlElement("device_info")]
        public String DeviceInfo { get; set; }

        /// <summary>
        /// 用户在商户appid下的唯一标识
        /// </summary>
        [XmlElement("openid")]
        public String OpenId { get; set; }

        /// <summary>
        /// 是否关注公众账号
        /// </summary>
        [XmlElement("is_subscribe")]
        public String IsSubscribe { get; set; }

        /// <summary>
        /// 调用接口提交的交易类型，取值如下：JSAPI，NATIVE，APP，MICROPAY，
        /// </summary>
        [XmlElement("trade_type")]
        public String TradeType { get; set; }

        /// <summary>
        /// SUCCESS—支付成功
        /// REFUND—转入退款
        /// NOTPAY—未支付
        /// CLOSED—已关闭
        /// REVOKED—已撤销（刷卡支付）
        /// USERPAYING--用户支付中
        /// PAYERROR--支付失败(其他原因，如银行返回失败)
        /// </summary>
        [XmlElement("trade_state")]
        public String TradeState { get; set; }

        /// <summary>
        /// 银行类型，采用字符串类型的银行标识
        /// </summary>
        [XmlElement("total_fee")]
        public String TotalFee { get; set; }

        /// <summary>
        /// 当订单使用了免充值型优惠券后返回该参数，
        /// 应结订单金额=订单金额-免充值优惠券金额。
        /// </summary>
        [XmlElement("settlement_total_fee")]
        public String SettlementTotalFee { get; set; }

        /// <summary>
        /// 货币类型，符合ISO 4217标准的三位字母代码，
        /// 默认人民币：CNY
        /// </summary>
        [XmlElement("fee_type")]
        public String FeeType { get; set; }

        /// <summary>
        /// 现金支付金额订单现金支付金额
        /// </summary>
        [XmlElement("cash_fee")]
        public String CashFee { get; set; }

        /// <summary>
        /// 货币类型，符合ISO 4217标准的三位字母代码，默认人民币：CNY
        /// </summary>
        [XmlElement("cash_fee_type")]
        public String CashFeeType { get; set; }

        /// <summary>
        /// “代金券”金额<=订单金额，订单金额-“代金券”金额=现金支付金额
        /// </summary>
        [XmlElement("coupon_fee")]
        public String CouponFee { get; set; }

        /// <summary>
        /// 代金券使用数量
        /// </summary>
        [XmlElement("coupon_count")]
        public String CouponCount { get; set; }

        /// <summary>
        /// CASH--充值代金券
        /// NO_CASH---非充值优惠券
        /// 开通免充值券功能，并且订单使用了优惠券后有返回（取值：CASH、NO_CASH）。
        /// $n为下标,从0开始编号，举例：coupon_type_$0
        /// </summary>
        [XmlElement("coupon_type_$n")]
        public String CouponTypeN { get; set; }

        /// <summary>
        /// 代金券ID, $n为下标，从0开始编号
        /// </summary>
        [XmlElement("coupon_id_$n")]
        public String CouponIdN { get; set; }

        /// <summary>
        /// 单个代金券支付金额, $n为下标，从0开始编号
        /// </summary>
        [XmlElement("coupon_fee_$n")]
        public String CouponFeeN { get; set; }

        /// <summary>
        /// 微信支付订单号
        /// </summary>
        [XmlElement("transaction_id")]
        public String WxTradeNo { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        [XmlElement("out_trade_no")]
        public String TradeNo { get; set; }

        /// <summary>
        /// 微信支付订单号
        /// </summary>
        [XmlElement("attach")]
        public String Attach { get; set; }

        /// <summary>
        /// 微信支付订单号
        /// </summary>
        [XmlElement("time_end")]
        public String FinishTime { get; set; }

        /// <summary>
        /// 对当前查询订单状态的描述和下一步操作的指引
        /// </summary>
        [XmlElement("trade_state_desc")]
        public String TradeStateDesc { get; set; }

    }
}
