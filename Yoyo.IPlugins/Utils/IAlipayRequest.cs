using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Utils
{
    /// <summary>
    /// 支付宝支付
    /// </summary>
    /// <typeparam name="AlipayResult"></typeparam>
    public interface IAlipayRequest<out AlipayResult>
    {
        /// <summary>
        /// Method
        /// </summary>
        /// <returns></returns>
        String GetApiName();

        /// <summary>
        /// 公共请求参数
        /// </summary>
        /// <returns></returns>
        UtilDictionary GetPublicParam();

        /// <summary>
        /// 请求参数
        /// </summary>
        /// <returns></returns>
        UtilDictionary GetParam();
    }
}
