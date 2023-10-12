using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Yoyo.IPlugins.Utils;

namespace Yoyo.IPlugins.Request
{
    /// <summary>
    /// 删除文件
    /// </summary>
    public class ReqCosDelObject : IQCloudRequest<Response.RspCosDelObject>
    {
        private String FilePath;
        private StreamContent Content;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="path">文件路径：/开始</param>
        public ReqCosDelObject(String path)
        {
            this.FilePath = path;
        }

        /// <summary>
        /// 请求方式
        /// </summary>
        /// <returns></returns>
        public Enums.QCloudMethod GetMethod()
        {
            return Enums.QCloudMethod.delete;
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
