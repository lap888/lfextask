using System;
using System.Collections.Generic;
using System.Text;
using Yoyo.IPlugins.Utils;

namespace Yoyo.IPlugins.Request
{
    /// <summary>
    /// 查询退款
    /// </summary>
    public class ReqAlipayRefundQuery : IAlipayRequest<AlipayResult<Response.RspAlipayRefundQuery>>
    {
        /// <summary>
        /// 【二选一】订单支付时传入的商户订单号,和支付宝交易号不能同时为空。
        /// trade_no,out_trade_no如果同时存在优先取trade_no
        /// </summary>
        public String OutTradeNo { get; set; }

        /// <summary>
        /// 【二选一】支付宝交易号，和商户订单号不能同时为空
        /// </summary>
        public String TradeNo { get; set; }

        /// <summary>
        /// 【必选】请求退款接口时，传入的退款请求号，如果在退款请求时未传入，则该值为创建交易时的外部交易号
        /// </summary>
        public String OutRequestNo { get; set; }

        /// <summary>
        /// 银行间联模式下有用，其它场景请不要使用；
        /// 双联通过该参数指定需要查询的交易所属收单机构的pid;
        /// </summary>
        public String OrgPid { get; set; }

        /// <summary>
        /// Method
        /// </summary>
        /// <returns></returns>
        public String GetApiName()
        {
            return "alipay.trade.fastpay.refund.query";
        }

        /// <summary>
        /// 请求参数
        /// </summary>
        /// <returns></returns>
        public UtilDictionary GetParam()
        {
            UtilDictionary Param = new UtilDictionary();
            Param.Add("trade_no", this.TradeNo);
            Param.Add("out_trade_no", this.OutTradeNo);
            Param.Add("out_request_no", this.OutRequestNo);
            Param.Add("org_pid", this.OrgPid);

            return Param;
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

            return PublicParam;
        }
    }
}
