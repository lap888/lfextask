using System;
using System.Collections.Generic;
using System.Text;
using Yoyo.IPlugins.Utils;

namespace Yoyo.IPlugins.Request
{
    /// <summary>
    /// 
    /// </summary>
    public class ReqAlipayWapSubmit : IAlipayRequest<AlipayResult<Response.RspAlipayWapSubmit>>
    {
        #region 公共请求参数
        /// <summary>
        /// 回跳地址
        /// </summary>
        public String ReturnUrl { get; set; }

        /// <summary>
        /// 支付宝服务器主动通知商户服务器里指定的页面http/https路径。
        /// </summary>
        public String NotifyUrl { get; set; }

        /// <summary>
        /// OAuth 2.0 Token
        /// </summary>
        public String AppAuthToken { get; set; }
        #endregion

        /// <summary>
        /// 该笔订单允许的最晚付款时间，逾期将关闭交易。
        /// 取值范围：1m～15d。m-分钟，h-小时，d-天，1c-当天
        /// （1c-当天的情况下，无论交易何时创建，都在0点关闭）。 
        /// 该参数数值不接受小数点， 如 1.5h，可转换为 90m。
        /// </summary>
        public String TimeOutExpress { get; set; }

        /// <summary>
        /// 绝对超时时间，格式为yyyy-MM-dd HH:mm。
        /// </summary>
        public String TimeExpire { get; set; }

        /// <summary>
        /// 【必选】订单总金额，单位为元，精确到小数点后两位
        /// 取值范围[0.01,100000000]
        /// </summary>
        public String TotalAmount { get; set; }

        /// <summary>
        /// 销售产品码，商家和支付宝签约的产品码
        /// </summary>
        public String ProductCode { get; private set; } = "QUICK_WAP_WAY";

        /// <summary>
        /// 对一笔交易的具体描述信息。
        /// 如果是多种商品，请将商品描述字符串累加传给body。
        /// </summary>
        public String Body { get; set; }

        /// <summary>
        /// 【必选】商品的标题/交易标题/订单标题/订单关键字等。
        /// </summary>
        public String Subject { get; set; }

        /// <summary>
        /// 【必选】商户网站唯一订单号
        /// </summary>
        public String OutTradeNo { get; set; }

        /// <summary>
        /// 针对用户授权接口，获取用户相关数据时，用于标识用户授权关系
        /// </summary>
        public String AuthToken { get; set; }

        /// <summary>
        /// 商品主类型 :0-虚拟类商品,1-实物类商品
        /// </summary>
        public String GoodsType { get; set; }

        /// <summary>
        /// 优惠参数
        /// 注：仅与支付宝协商后可用
        /// {"storeIdType":"1"}
        /// </summary>
        public String PromoParams { get; set; }

        /// <summary>
        /// 公用回传参数，如果请求时传递了该参数，则返回给商户时会回传该参数。
        /// 支付宝只会在同步返回（包括跳转回商户网站）和异步通知时将该参数原样返回。
        /// 本参数必须进行UrlEncode之后才可以发送给支付宝。
        /// </summary>
        public String PassbackParams { get; set; }

        /// <summary>
        /// 用户付款中途退出返回商户网站的地址
        /// </summary>
        public String QuitUrl { get; set; }

        /// <summary>
        /// 业务扩展参数
        /// </summary>
        public UtilDictionary ExtendParams { get; set; }

        /// <summary>
        /// 商户原始订单号，最大长度限制32位
        /// </summary>
        public String MerchantOrderNo { get; set; }

        /// <summary>
        /// 指定渠道，目前仅支持传入pcredit
        /// 若由于用户原因渠道不可用，用户可选择是否用其他渠道支付。
        /// 注：该参数不可与花呗分期参数同时传入
        /// </summary>
        public String SpecifiedChannel { get; set; }

        /// <summary>
        /// 可用渠道，用户只能在指定渠道范围内支付
        /// 当有多个渠道时用“,”分隔
        /// 注，与disable_pay_channels互斥
        /// 示例：pcredit,moneyFund,debitCardExpress
        /// </summary>
        public String EnablePayChannels { get; set; }

        /// <summary>
        /// 禁用渠道，用户不可用指定渠道支付
        /// 当有多个渠道时用“,”分隔
        /// 注，与enable_pay_channels互斥
        /// </summary>
        public String DisablePayChannels { get; set; }

        /// <summary>
        /// 商户传入业务信息，具体值要和支付宝约定，应用于安全，营销等参数直传场景，格式为json格式
        /// </summary>
        public UtilDictionary BusinessParams { get; set; }

        /// <summary>
        /// 签约参数。如果希望在sdk中支付并签约，需要在这里传入签约信息。
        /// </summary>
        public UtilDictionary AgreementSignParams { get; set; }

        /// <summary>
        /// 外部指定买家
        /// </summary>
        public UtilDictionary ExtUserInfo { get; set; }

        /// <summary>
        /// 商户门店编号
        /// </summary>
        public String StoreId { get; set; }

        public String GetApiName()
        {
            return "alipay.trade.wap.pay";
        }

        /// <summary>
        /// 获取公共请求参数
        /// </summary>
        /// <returns></returns>
        public UtilDictionary GetPublicParam()
        {
            UtilDictionary PublicParam = new UtilDictionary();

            PublicParam.Add("method", this.GetApiName());
            PublicParam.Add("version", "1.0");
            PublicParam.Add("return_url", this.ReturnUrl);
            PublicParam.Add("notify_url", this.NotifyUrl);
            PublicParam.Add("app_auth_token", this.AppAuthToken);
            return PublicParam;
        }

        /// <summary>
        /// 获取请求参数
        /// </summary>
        /// <returns></returns>
        public UtilDictionary GetParam()
        {
            UtilDictionary Param = new UtilDictionary();
            Param.Add("total_amount", this.TotalAmount);
            Param.Add("product_code", this.ProductCode);
            Param.Add("body", this.Body);
            Param.Add("subject", this.Subject);
            Param.Add("out_trade_no", this.OutTradeNo);
            Param.Add("auth_token", this.AuthToken);

            Param.Add("timeout_express", this.TimeOutExpress);
            Param.Add("time_expire", this.TimeExpire);
            Param.Add("goods_type", this.GoodsType);
            Param.Add("promo_params", this.PromoParams);
            Param.Add("passback_params", this.PromoParams);
            Param.Add("quit_url", this.QuitUrl);
            Param.Add("extend_params", this.ExtendParams);
            Param.Add("merchant_order_no", this.MerchantOrderNo);
            Param.Add("specified_channel", this.SpecifiedChannel);
            Param.Add("enable_pay_channels", this.EnablePayChannels);
            Param.Add("disable_pay_channels", this.DisablePayChannels);
            Param.Add("store_id", this.StoreId);
            Param.Add("ext_user_info", this.ExtUserInfo);
            Param.Add("business_params", this.BusinessParams);
            Param.Add("agreement_sign_params", this.AgreementSignParams);

            return Param;
        }
    }
}
