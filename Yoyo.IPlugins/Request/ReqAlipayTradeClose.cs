using System;
using System.Collections.Generic;
using System.Text;
using Yoyo.IPlugins.Utils;

namespace Yoyo.IPlugins.Request
{
    /// <summary>
    /// 关闭订单
    /// </summary>
    public class ReqAlipayTradeClose : IAlipayRequest<AlipayResult<Response.RspAlipayTradeClose>>
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
        /// 卖家端自定义的的操作员 ID
        /// </summary>
        public String OperatorId { get; set; }

        /// <summary>
        /// Method
        /// </summary>
        /// <returns></returns>
        public String GetApiName()
        {
            return "alipay.trade.close";
        }


        /// <summary>
        /// 请求参数
        /// </summary>
        /// <returns></returns>
        public UtilDictionary GetParam()
        {
            UtilDictionary Param = new UtilDictionary();
            Param.Add("trade_no",this.TradeNo);
            Param.Add("out_trade_no", this.OutTradeNo);
            Param.Add("operator_id", this.OperatorId);
            return Param;
        }

        /// <summary>
        /// 公共请求参数
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
