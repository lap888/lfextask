using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Request
{
    /// <summary>
    /// 发起认证
    /// </summary>
    public class ReqRealVerifyInitiate : Utils.IVerifyRequest<Response.RspRealVerifyInitiate>
    {
        /// <summary>
        /// 发起认证请求的操作。
        /// 取值必须是init。
        /// </summary>
        public String Method { get; private set; } = "init";

        /// <summary>
        /// 认证场景ID，在控制台创建认证场景后自动生成。
        /// </summary>
        public String SceneId { get; set; }

        /// <summary>
        /// 商户请求的唯一标识，值为32位长度的字母数字组合前面几位字符是商户自定义的简称，中间可以使用一段时间，后段可以使用一个随机或递增序列。
        /// </summary>
        public String OuterOrderNo { get; set; }

        /// <summary>
        /// 认证场景码和用户发起认证的端有关：
        /// 当用户在 iOS 或安卓平台发起认证时，认证场景码是 FACE_SDK。
        /// 当用户在小程序中或 H5 页面中发起认证时，认证场景码是 FACE。
        /// </summary>
        public Enums.RealVerifyBizCode BizCode { get; set; }

        /// <summary>
        /// 身份信息参数类型，必须传入 CERT_INFO。
        /// </summary>
        public String IdentityType { get; private set; } = "CERT_INFO";

        /// <summary>
        /// 证件类型，当前支持身份证，必须传入IDENTITY_CARD。
        /// </summary>
        public String CertType { get; private set; } = "IDENTITY_CARD";

        /// <summary>
        /// 用户身份证件号。
        /// </summary>
        public String CertNo { get; set; }

        /// <summary>
        /// 用户姓名。
        /// </summary>
        public String CertName { get; set; }

        /// <summary>
        /// 商户业务页面回调的目标地址。
        /// 如您不需要回调商户业务页面，您可以在此处传入空字符串。
        /// 当您采用端外唤起支付宝认证页面接入，且希望您的用户唤起支付宝完成认证后，能够跳回您的应用页面，您需要在此参数下传入您的应用的 Scheme。
        /// 详情请参看：回跳原应用。
        /// </summary>
        public String ReturnUrl { get; set; }

        /// <summary>
        /// 获取请求参数
        /// </summary>
        /// <returns></returns>
        public Utils.UtilDictionary GetPairs()
        {
            Utils.UtilDictionary Pairs = new Utils.UtilDictionary();
            Pairs.Add("method", this.Method);
            Pairs.Add("sceneId", this.SceneId);
            Pairs.Add("outerOrderNo", this.OuterOrderNo);
            Pairs.Add("bizCode", this.BizCode.ToString());
            Pairs.Add("identityType", this.IdentityType);
            Pairs.Add("certType", this.CertType);
            Pairs.Add("certNo", this.CertNo);
            Pairs.Add("certName", this.CertName);
            Pairs.Add("returnUrl", this.ReturnUrl);

            return Pairs;
        }
    }
}
