using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IPlugins.Utils
{
    /// <summary>
    /// 请求接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IWePayRequest<out T> where T : WePayResponse, new()
    {
        /// <summary>
        /// 获取请求地址
        /// </summary>
        /// <returns></returns>
        String GetUrl();

        /// <summary>
        /// 获取请求参数
        /// </summary>
        /// <returns></returns>
        WeXmlDoc GetXmlDoc();

    }
}
