using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yoyo.IPlugins
{
    /// <summary>
    /// 微信支付
    /// </summary>
    public interface IWeChatPay
    {
        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<T> Execute<T>(Utils.IWePayRequest<T> request) where T : Utils.WePayResponse, new();
    }
}
