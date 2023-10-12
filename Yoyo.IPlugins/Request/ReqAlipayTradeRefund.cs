using System;
using System.Collections.Generic;
using System.Text;
using Yoyo.IPlugins.Utils;

namespace Yoyo.IPlugins.Request
{
    public class ReqAlipayTradeRefund : IAlipayRequest<AlipayResult<Response.RspAlipayTradeRefund>>
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
        /// 【必选】	
        /// </summary>
        public Decimal RefundAmount { get; set; }

        /// <summary>
        /// 订单退款币种信息	
        /// </summary>
        public String RefundCurrency { get; set; }

        /// <summary>
        /// 退款的原因说明
        /// </summary>
        public String RefundReason { get; set; }

        /// <summary>
        /// 标识一次退款请求，同一笔交易多次退款需要保证唯一，如需部分退款，则此参数必传。
        /// </summary>
        public String OutRequestNo { get; set; }

        /// <summary>
        /// 商户的操作员编号
        /// </summary>
        public String OperatorId { get; set; }

        /// <summary>
        /// 商户的门店编号
        /// </summary>
        public String StoreId { get; set; }

        /// <summary>
        /// 商户的终端编号
        /// </summary>
        public String TerminalId { get; set; }

        /// <summary>
        /// 银行间联模式下有用，其它场景请不要使用；
        /// 双联通过该参数指定需要退款的交易所属收单机构的pid;
        /// </summary>
        public String OrgPid { get; set; }

        /// <summary>
        /// 退款包含的商品列表信息，Json格式。
        /// 其它说明详见：“商品明细说明”
        /// </summary>
        public List<UtilDictionary> GoodsDetails { get; set; }

        /// <summary>
        /// 退分账明细信息
        /// </summary>
        public List<UtilDictionary> RefundRoyaltyParameters { get; set; }

        /// <summary>
        /// Method
        /// </summary>
        /// <returns></returns>
        public String GetApiName()
        {
            return "alipay.trade.refund";
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
            Param.Add("refund_amount", this.RefundAmount);
            Param.Add("refund_currency", this.RefundCurrency);
            Param.Add("refund_reason", this.RefundReason);
            Param.Add("out_request_no", this.OutRequestNo);
            Param.Add("operator_id", this.OperatorId);
            Param.Add("store_id", this.StoreId);
            Param.Add("terminal_id", this.TerminalId);
            Param.Add("goods_detail", this.GoodsDetails);
            Param.Add("refund_royalty_parameters", this.RefundRoyaltyParameters);
            Param.Add("org_pid", this.OrgPid);

            return Param;
        }

        /// <summary>
        /// 请求公共参数
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
