using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Utils
{
    /// <summary>
    /// 短信请求模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISMSRequest<out T> where T : SMSResponse, new()
    {
        /// <summary>
        /// 获取请求路径
        /// </summary>
        /// <returns></returns>
        String GetUrl();
    }
}
