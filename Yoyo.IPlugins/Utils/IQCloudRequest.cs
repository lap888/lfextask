using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Yoyo.IPlugins.Utils
{
    /// <summary>
    /// 腾讯云请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQCloudRequest<out T> where T : QCloudResponse, new() 
    {
        /// <summary>
        /// 请求方式
        /// </summary>
        /// <returns></returns>
        Enums.QCloudMethod GetMethod();

        /// <summary>
        /// 路径
        /// </summary>
        String GetPath();

        /// <summary>
        /// 获取请求内容
        /// </summary>
        /// <returns></returns>
        HttpContent GetContent();

        /// <summary>
        /// HttpHeaders
        /// </summary>
        /// <returns></returns>
        UtilDictionary GetHeaderParam();

        /// <summary>
        /// 请求参数
        /// </summary>
        /// <returns></returns>
        UtilDictionary GetHttpParam();
    }
}
