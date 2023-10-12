using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Utils
{
    /// <summary>
    /// 实名认证
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IVerifyRequest<out T> where T : RealVerifyResponse, new()
    {
        /// <summary>
        /// 获取请求参数
        /// </summary>
        /// <returns></returns>
        UtilDictionary GetPairs();
    }
}
