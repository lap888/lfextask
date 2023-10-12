using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yoyo.IPlugins.Utils;

namespace Yoyo.IPlugins
{
    /// <summary>
    /// 支付宝支付
    /// </summary>
    public interface IAlipayPlugin
    {
        /// <summary>
        /// 执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<AlipayResult<T>> Execute<T>(IAlipayRequest<AlipayResult<T>> request) where T : AlipayResponse, new();

        /// <summary>
        /// 获取APP签名串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<String> GetSignStr<T>(IAlipayRequest<AlipayResult<T>> request) where T : AlipayResponse, new();


    }
}
