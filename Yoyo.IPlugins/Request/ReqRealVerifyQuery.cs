using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Request
{
    /// <summary>
    /// 查询认证结果
    /// </summary>
    public class ReqRealVerifyQuery : Utils.IVerifyRequest<Response.RspRealVerifyQuery>
    {
        /// <summary>
        /// 查询人脸比对结果的操作。
        /// 取值必须为query。
        /// </summary>
        public String Method { get; private set; } = "query";

        /// <summary>
        /// 认证场景ID，需与发起认证请求时的sceneId保持一致。
        /// </summary>
        public String SceneId { get; set; }

        /// <summary>
        /// 认证ID，需与发起认证请求时返回的certifyId保持一致。
        /// </summary>
        public String CertifyId { get; set; }

        /// <summary>
        /// 获取请求参数
        /// </summary>
        /// <returns></returns>
        public Utils.UtilDictionary GetPairs()
        {
            Utils.UtilDictionary Pairs = new Utils.UtilDictionary();
            Pairs.Add("method", this.Method);
            Pairs.Add("sceneId", this.SceneId);
            Pairs.Add("certifyId", this.CertifyId);

            return Pairs;
        }
    }
}
