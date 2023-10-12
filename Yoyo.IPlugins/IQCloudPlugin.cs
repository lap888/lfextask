using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Yoyo.IPlugins.Utils;

namespace Yoyo.IPlugins
{
    /// <summary>
    /// 腾讯云接口
    /// </summary>
    public interface IQCloudPlugin
    {
        /// <summary>
        /// 腾讯云请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="minute">有效时长，分钟</param>
        /// <returns></returns>
        //Task<T> Execute<T>(IQCloudRequest<T> request, Int32 minute) where T : QCloudResponse, new();

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name=""></param>
        /// <returns></returns>
        Task<Boolean> PutObject(String path, FileStream file);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<Stream> GetObject(String path);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<Boolean> DelObject(String path);
        
        /// <summary>
        /// 获取Js临时授权
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<String> PutAuthStr(String path);
    }
}
