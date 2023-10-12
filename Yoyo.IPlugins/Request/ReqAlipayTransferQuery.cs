using System;
using System.Collections.Generic;
using System.Text;
using Yoyo.IPlugins.Utils;

namespace Yoyo.IPlugins.Request
{
    /// <summary>
    /// 转账业务单据查询接口
    /// </summary>
    public class ReqAlipayTransferQuery : IAlipayRequest<AlipayResult<Response.RspAlipayTransferQuery>>
    {
        #region 公共请求参数
        /// <summary>
        /// OAuth 2.0 Token
        /// </summary>
        public String AppAuthToken { get; set; }
        #endregion

        /// <summary>
        /// 销售产品码，商家和支付宝签约的产品码，如果传递了out_biz_no则该字段为必传。可传值如下：
        /// STD_RED_PACKET：现金红包
        /// TRANS_ACCOUNT_NO_PWD：单笔无密转账
        /// </summary>
        public String ProductCode { get; set; }

        /// <summary>
        /// 描述特定的业务场景，如果传递了out_biz_no则该字段为必传。可取的业务场景如下：
        /// PERSONAL_PAY：C2C现金红包-发红包；
        /// PERSONAL_COLLECTION：C2C现金红包-领红包；
        /// REFUND：C2C现金红包-红包退回；
        /// DIRECT_TRANSFER：B2C现金红包、单笔无密转账
        /// </summary>
        public String BizScene { get; set; }

        /// <summary>
        /// 商户转账唯一订单号：发起转账来源方定义的转账单据ID。
        /// 本参数和支付宝转账单据号、支付宝支付资金流水号三者不能同时为空。
        /// 当本参数和支付宝转账单据号、支付宝支付资金流水号同时提供时，将用支付宝支付资金流水号进行查询，忽略本参数；
        /// 当本参数和支付宝转账单据号同时提供时，将用支付宝转账单据号进行查询，忽略本参数；
        /// </summary>
        public String OutBizNo { get; set; }

        /// <summary>
        /// 支付宝转账单据号：
        /// 本参数和商户转账唯一订单号、支付宝支付资金流水号三者不能同时为空。
        /// 当本参数和商户转账唯一订单号、支付宝支付资金流水号三者同时提供时，将用支付宝支付资金流水号进行查询，忽略其余两者；
        /// 当本参数和支付宝支付资金流水号同时提供时，将用支付宝支付资金流水号进行查询，忽略本参数。
        /// 当本参数和商户转账唯一订单号同时提供时，将用本参数进行查询，忽略商户转账唯一订单号。
        /// </summary>
        public String OrderId { get; set; }

        /// <summary>
        /// 支付宝支付资金流水号：
        /// 本参数和支付宝转账单据号、商户转账唯一订单号三者不能同时为空。
        /// 当本参数和支付宝转账单据号、商户转账唯一订单号同时提供时，将用本参数进行查询，忽略本参数；
        /// 当本参数和支付宝转账单据号同时提供时，将用本参数进行查询，忽略支付宝转账单据号；
        /// 当本参数和商户转账唯一订单号同时提供时，将用本参数进行查询，忽略商户转账唯一订单号；
        /// </summary>
        public String PayFundOrderId { get; set; }

        /// <summary>
        /// 接口名称
        /// </summary>
        /// <returns></returns>
        public String GetApiName()
        {
            return "alipay.fund.trans.common.query";
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

            Param.Add("product_code", this.ProductCode);
            Param.Add("biz_scene", this.BizScene);
            Param.Add("out_biz_no", this.OutBizNo);
            Param.Add("order_id", this.OrderId);
            Param.Add("pay_fund_order_id", this.PayFundOrderId);

            return Param;
        }
    }
}
