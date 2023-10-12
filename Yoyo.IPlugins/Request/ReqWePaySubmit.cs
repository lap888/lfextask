using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Request
{
    /// <summary>
    /// 统一下单
    /// </summary>
    public class ReqWePaySubmit : Utils.IWePayRequest<Response.RspWePaySubmit>
    {
        /// <summary>
        /// 获取请求地址
        /// </summary>
        /// <returns></returns>
        public String GetUrl()
        {
            return "https://api.mch.weixin.qq.com/pay/unifiedorder";
        }

        /// <summary>
        /// 微信分配的小程序ID
        /// </summary>
        public String AppId { get; set; }

        /// <summary>
        /// 微信支付分配的商户号
        /// </summary>
        public String MchId { get; set; }

        /// <summary>
        /// 自定义参数，可以为终端设备号(门店号或收银设备ID)，PC网页或公众号内支付可以传"WEB"
        /// </summary>
        public String DeviceInfo { get; set; }

        /// <summary>
        /// 随机字符串，长度要求在32位以内。
        /// </summary>
        public String NonceStr { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public String Sign { get; set; }

        /// <summary>
        /// 签名类型，默认为MD5，支持HMAC-SHA256和MD5。
        /// </summary>
        public String SignType { get; set; } = "MD5";

        /// <summary>
        /// 商品简单描述，该字段请按照规范传递
        /// </summary>
        public String Body { get; set; }

        /// <summary>
        /// 商品详细描述，对于使用单品优惠的商户
        /// </summary>
        public String Detail { get; set; }

        /// <summary>
        /// 附加数据，在查询API和支付通知中原样返回，可作为自定义参数使用。
        /// </summary>
        public String Attach { get; set; }

        /// <summary>
        /// 商户系统内部订单号，要求32个字符内，只能是数字、大小写字母_-|*且在同一个商户号下唯一。
        /// </summary>
        public String TradeNo { get; set; }

        /// <summary>
        /// 符合ISO 4217标准的三位字母代码，默认人民币：CNY
        /// </summary>
        public String FeeType { get; set; } = "CNY";

        /// <summary>
        /// 订单总金额，单位为分
        /// </summary>
        public Int32 TotalFee { get; set; }

        /// <summary>
        /// 支持IPV4和IPV6两种格式的IP地址。调用微信支付API的机器IP
        /// </summary>
        public String Ip { get; set; }

        /// <summary>
        /// 订单生成时间，格式为yyyyMMddHHmmss，
        /// 如2009年12月25日9点10分10秒表示为20091225091010。
        /// </summary>
        public String TimeStart { get; set; }

        /// <summary>
        /// 交易结束时间
        /// </summary>
        public String TimeExpire { get; set; }

        /// <summary>
        /// 订单优惠标记，使用代金券或立减优惠功能时需要的参数
        /// </summary>
        public String GoodsTag { get; set; }

        /// <summary>
        /// 异步接收微信支付结果通知的回调地址，
        /// 通知url必须为外网可访问的url，
        /// 不能携带参数。
        /// </summary>
        public String NotifyUrl { get; set; }

        /// <summary>
        /// 小程序取值如下：JSAPI，
        /// </summary>
        public String TradeType { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public String ProductId { get; set; }

        /// <summary>
        /// 上传此参数no_credit--可限制用户不能使用信用卡支付
        /// </summary>
        public String LimitPay { get; set; }

        /// <summary>
        /// trade_type=JSAPI，此参数必传，用户在商户appid下的唯一标识。
        /// </summary>
        public String OpenId { get; set; }

        /// <summary>
        /// Y，传入Y时，支付成功消息和支付详情页将出现开票入口。
        /// 需要在微信支付商户平台或微信公众平台开通电子发票功能，
        /// 传此字段才可生效
        /// </summary>
        public String Receipt { get; set; }

        /// <summary>
        /// 该字段为JSON对象数据
        /// </summary>
        public SceneInfo SceneInfo { get; set; }

        /// <summary>
        /// 获取请求参数
        /// </summary>
        /// <returns></returns>
        public Utils.WeXmlDoc GetXmlDoc()
        {
            Utils.WeXmlDoc XmlDoc = new Utils.WeXmlDoc();

            XmlDoc.Add("appid", this.AppId);
            XmlDoc.Add("mch_id", this.MchId);
            XmlDoc.Add("device_info", this.DeviceInfo);
            XmlDoc.Add("nonce_str", this.NonceStr);
            XmlDoc.Add("sign_type", this.SignType);
            XmlDoc.Add("body", this.Body);
            XmlDoc.Add("attach", this.Attach);
            XmlDoc.Add("out_trade_no", this.TradeNo);

            XmlDoc.Add("fee_type", this.FeeType);
            XmlDoc.Add("total_fee", this.TotalFee);
            XmlDoc.Add("spbill_create_ip", this.Ip);
            XmlDoc.Add("time_start", this.TimeStart);
            XmlDoc.Add("time_expire", this.TimeExpire);

            XmlDoc.Add("goods_tag", this.GoodsTag);
            XmlDoc.Add("notify_url", this.NotifyUrl);
            XmlDoc.Add("trade_type", this.TradeType);
            XmlDoc.Add("product_id", this.ProductId);
            XmlDoc.Add("limit_pay", this.LimitPay);
            XmlDoc.Add("openid", this.OpenId);
            XmlDoc.Add("receipt", this.Receipt);

            if (this.SceneInfo != null)
            {
                XmlDoc.Add("scene_info", JsonConvert.SerializeObject(this.SceneInfo));
            }
            XmlDoc.Add("detail", this.Detail);
            return XmlDoc;
        }
    }

    /// <summary>
    /// 该字段常用于线下活动时的场景信息上报，支持上报实际门店信息，商户也可以按需求自己上报相关信息。
    /// 该字段为JSON对象数据，对象格式为{"store_info":{"id": "门店ID","name": "名称","area_code": "编码","address": "地址" }}
    /// </summary>
    public class SceneInfo
    {
        /// <summary>
        /// 门店编号，由商户自定义
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// 门店名称 ，由商户自定义
        /// </summary>
        [JsonProperty("name")]
        public int Name { get; set; }

        /// <summary>
        /// 门店所在地行政区划码
        /// </summary>
        [JsonProperty("area_code")]
        public int AreaCode { get; set; }

        /// <summary>
        /// 门店详细地址 ，由商户自定义
        /// </summary>
        [JsonProperty("address")]
        public int Address { get; set; }
    }

}
