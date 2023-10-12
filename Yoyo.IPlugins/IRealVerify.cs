using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yoyo.IPlugins
{
    /// <summary>
    /// 实名验证
    /// </summary>
    public interface IRealVerify
    {
        /// <summary>
        /// 执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<T> Execute<T>(Utils.IVerifyRequest<T> request) where T : Utils.RealVerifyResponse, new();
    }
}
