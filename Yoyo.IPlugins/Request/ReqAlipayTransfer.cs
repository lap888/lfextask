using System;
using System.Collections.Generic;
using System.Text;
using Yoyo.IPlugins.Utils;

namespace Yoyo.IPlugins.Request
{
    /// <summary>
    /// 单笔转账接口
    /// </summary>
    public class ReqAlipayTransfer : IAlipayRequest<AlipayResult<Response.RspAlipayTransfer>>
    {
        #region 公共请求参数
        /// <summary>
        /// OAuth 2.0 Token
        /// </summary>
        public String AppAuthToken { get; set; }
        #endregion

        /// <summary>
        /// 【必选】商户端的唯一订单号，对于同一笔转账请求，商户需保证该订单号唯一。
        /// </summary>
        public String OutBizNo { get; set; }

        /// <summary>
        /// 【必选】订单总金额，单位为元，精确到小数点后两位，
        /// STD_RED_PACKET产品取值范围[0.01,100000000]；
        /// TRANS_ACCOUNT_NO_PWD产品取值范围[0.1, 100000000]
        /// </summary>
        public Decimal TransAmount { get; set; }

        /// <summary>
        /// 【必选】业务产品码，
        /// 收发现金红包固定为：STD_RED_PACKET；
        /// 单笔无密转账到支付宝账户固定为：TRANS_ACCOUNT_NO_PWD；
        /// 单笔无密转账到银行卡固定为：TRANS_BANKCARD_NO_PWD
        /// </summary>
        public String ProductCode { get; set; }

        /// <summary>
        /// 描述特定的业务场景，可传的参数如下：
        /// PERSONAL_COLLECTION：C2C现金红包-领红包；
        /// DIRECT_TRANSFER：B2C现金红包、单笔无密转账到支付宝/银行卡
        /// </summary>
        public String BizScene { get; set; }

        /// <summary>
        /// 转账业务的标题，用于在支付宝用户的账单里显示
        /// </summary>
        public String OrderTitle { get; set; }

        /// <summary>
        /// 原支付宝业务单号。
        /// C2C现金红包-红包领取时，传红包支付时返回的支付宝单号；
        /// B2C现金红包、单笔无密转账到支付宝/银行卡不需要该参数。
        /// </summary>
        public String OriginalOrderId { get; set; }

        /// <summary>
        /// 【必选】参与方的唯一标识
        /// </summary>
        public String Identity { get; set; }

        /// <summary>
        /// 参与方的标识类型，目前支持如下类型：
        /// 1、ALIPAY_USER_ID 支付宝的会员ID
        /// 2、ALIPAY_LOGON_ID：支付宝登录号，支持邮箱和手机号格式
        /// </summary>
        public String IdentityType { get; set; }

        /// <summary>
        /// 参与方真实姓名，如果非空，将校验收款支付宝账号姓名一致性。
        /// 当identity_type=ALIPAY_LOGON_ID时，本字段必填。
        /// </summary>
        public String TrueName { get; set; }

        /// <summary>
        /// 业务备注
        /// </summary>
        public String Remark { get; set; }

        /// <summary>
        /// 【红包业务必选】转账业务请求的扩展参数，支持传入的扩展参数如下：
        /// 1、sub_biz_scene 子业务场景，红包业务必传，取值REDPACKET，C2C现金红包、B2C现金红包均需传入；
        /// 2、withdraw_timeliness为转账到银行卡的预期到账时间，可选（不传入则默认为T1），
        /// 取值T0表示预期T+0到账，取值T1表示预期T+1到账，因到账时效受银行机构处理影响，支付宝无法保证一定是T0或者T1到账；
        /// {"sub_biz_scene":"REDPACKET"}
        /// </summary>
        public UtilDictionary BusinessParams { get; set; }

        /// <summary>
        /// 接口名称
        /// </summary>
        /// <returns></returns>
        public String GetApiName()
        {
            return "alipay.fund.trans.uni.transfer";
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
            UtilDictionary PayeeInfo = new UtilDictionary();
            PayeeInfo.Add("identity", this.Identity);
            PayeeInfo.Add("identity_type", this.IdentityType);
            PayeeInfo.Add("name", this.TrueName);

            Param.Add("out_biz_no", this.OutBizNo);
            Param.Add("trans_amount", this.TransAmount);
            Param.Add("product_code", this.ProductCode);
            Param.Add("biz_scene", this.BizScene);
            Param.Add("order_title", this.OrderTitle);
            Param.Add("original_order_id", this.OriginalOrderId);
            Param.Add("payee_info", PayeeInfo);
            Param.Add("remark", this.Remark);
            Param.Add("business_params", this.BusinessParams);

            return Param;
        }

    }
}
