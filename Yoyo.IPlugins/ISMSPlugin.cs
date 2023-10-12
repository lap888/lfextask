using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yoyo.IPlugins
{
    /// <summary>
    /// 短信插件接口
    /// </summary>
    public interface ISMSPlugin
    {
        /// <summary>
        /// 执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<T> Execute<T>(Utils.ISMSRequest<T> request) where T : Utils.SMSResponse, new();
    }
}
