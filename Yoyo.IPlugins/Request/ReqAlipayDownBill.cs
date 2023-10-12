using System;
using System.Collections.Generic;
using System.Text;
using Yoyo.IPlugins.Utils;

namespace Yoyo.IPlugins.Request
{
    public class ReqAlipayDownBill : IAlipayRequest<AlipayResult<Response.RspAlipayDownBill>>
    {
        #region 公共请求参数
        /// <summary>
        /// OAuth 2.0 Token
        /// </summary>
        public String AppAuthToken { get; set; }
        #endregion

        /// <summary>
        /// 账单类型，商户通过接口或商户经开放平台授权后其所属服务商通过接口可以获取以下账单类型：trade、signcustomer；
        /// trade指商户基于支付宝交易收单的业务账单；
        /// signcustomer是指基于商户支付宝余额收入及支出等资金变动的帐务账单。
        /// </summary>
        public String BillType { get; set; }

        /// <summary>
        /// 账单时间：日账单格式为yyyy-MM-dd，最早可下载2016年1月1日开始的日账单；
        /// 月账单格式为yyyy-MM，最早可下载2016年1月开始的月账单。
        /// </summary>
        public String BillDate { get; set; }

        public string GetApiName()
        {
            return "alipay.data.dataservice.bill.downloadurl.query";
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
            Param.Add("bill_type", this.BillType);
            Param.Add("bill_date", this.BillDate);
            return Param;
        }
    }
}
