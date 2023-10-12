using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Yoyo.IPlugins.Utils;

namespace Yoyo.IPlugins.Request
{
    /// <summary>
    /// 存入COS
    /// </summary>
    public class ReqCosPutObject : IQCloudRequest<Response.RspCosPutObject>
    {
        private String FilePath;
        private StreamContent Content;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="path">文件路径：/开始</param>
        public ReqCosPutObject(String path, StreamContent content)
        {
            this.FilePath = path;
            this.Content = content;
        }

        /// <summary>
        /// 请求方式
        /// </summary>
        /// <returns></returns>
        public Enums.QCloudMethod GetMethod()
        {
            return Enums.QCloudMethod.put;
        }

        /// <summary>
        /// 路径
        /// </summary>
        /// <returns></returns>
        public String GetPath()
        {
            return FilePath;
        }

        /// <summary>
        /// 请求体
        /// </summary>
        /// <returns></returns>
        public HttpContent GetContent()
        {
            return this.Content;
        }

        /// <summary>
        /// 获取头参数
        /// </summary>
        /// <returns></returns>
        public UtilDictionary GetHeaderParam()
        {
            UtilDictionary HeaderParam = new UtilDictionary();

            return HeaderParam;
        }

        /// <summary>
        /// 请求参数
        /// </summary>
        /// <returns></returns>
        public UtilDictionary GetHttpParam()
        {
            UtilDictionary Param = new UtilDictionary();

            return Param;
        }

    }
}
