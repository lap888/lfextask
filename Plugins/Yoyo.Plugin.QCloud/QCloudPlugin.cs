using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Yoyo.IPlugins;
using Yoyo.IPlugins.Utils;

namespace Yoyo.Plugin.IQCloud
{
    /// <summary>
    /// 腾讯云
    /// </summary>
    public class QCloudPlugin : IQCloudPlugin
    {
        private Models.Config config;
        private HttpClient client;
        public QCloudPlugin(IHttpClientFactory factory, IOptionsMonitor<Models.Config> monitor)
        {
            config = monitor.CurrentValue;
            client = factory.CreateClient(config.ClientName);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="minute">有效时长，分钟</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Execute<T>(IQCloudRequest<T> request, Int32 minute) where T : QCloudResponse, new()
        {
            String HostUrl = GetHost(this.config);
            String Path = request.GetPath();
            String AuthStr = GetAuthStr(request, minute, HostUrl, this.config.SecretId, this.config.SecretKey);
            String Url = $"https://{HostUrl}{Path}?{AuthStr}";

            HttpResponseMessage response = null;
            switch (request.GetMethod())
            {
                case IPlugins.Enums.QCloudMethod.put:
                    response = await client.PutAsync(Url, request.GetContent());
                    break;
                case IPlugins.Enums.QCloudMethod.get:
                    response = await client.GetAsync(Url);
                    break;
                case IPlugins.Enums.QCloudMethod.post:
                    response = await client.PostAsync(Url, request.GetContent());
                    break;
                case IPlugins.Enums.QCloudMethod.head:
                    break;
                case IPlugins.Enums.QCloudMethod.delete:
                    response = await client.DeleteAsync(Url);
                    break;
                case IPlugins.Enums.QCloudMethod.options:
                    break;
                default:
                    break;
            }

            return response;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<Boolean> PutObject(String path, FileStream file)
        {
            StreamContent content = new StreamContent(file);
            HttpResponseMessage response = await Execute(new IPlugins.Request.ReqCosPutObject(path, content), 20);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<Stream> GetObject(String path)
        {
            HttpResponseMessage response = await Execute(new IPlugins.Request.ReqCosGetObject(path), 20);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStreamAsync();
            }
            return null;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<Boolean> DelObject(String path)
        {
            HttpResponseMessage response = await Execute(new IPlugins.Request.ReqCosDelObject(path), 20);
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取Js临时授权
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<String> PutAuthStr(String path)
        {
            return await Task.Run(() =>
            {
                String HostUrl = GetHost(this.config);

                return GetAuthStr(new IPlugins.Request.ReqCosPutObject(path, null), 5, HostUrl, this.config.StsSecretId, this.config.StsSecretKey);
            });
        }

        #region 私有方法
        /// <summary>
        /// 制作认证串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="minute"></param>
        /// <returns></returns>
        private static String GetAuthStr<T>(IQCloudRequest<T> request, Int32 minute, String host, String secretId, String secretKey) where T : QCloudResponse, new()
        {
            String HostUrl = host;
            String Path = request.GetPath();
            IPlugins.Enums.QCloudMethod Method = request.GetMethod();

            UtilDictionary Headers = request.GetHeaderParam();
            Headers.Add("host", HostUrl);
            StringBuilder HeaderStr = new StringBuilder();
            Headers.Aggregate(HeaderStr, (s, i) => s.Append($"{i.Key}={HttpUtility.UrlEncode(i.Value, Encoding.UTF8)}&"));
            HeaderStr.Remove(HeaderStr.Length - 1, 1);

            UtilDictionary HttpParam = request.GetHttpParam();
            StringBuilder HttpParamStr = new StringBuilder();
            HttpParam.Aggregate(HttpParamStr, (s, i) => s.Append($"{i.Key}={HttpUtility.UrlEncode(i.Value, Encoding.UTF8)}&"));
            if (HttpParamStr.Length > 0)
            {
                HttpParamStr.Remove(HttpParamStr.Length - 1, 1);
            }

            String QKeyTime = $"{GetTimeStamp(DateTime.Now)};{GetTimeStamp(DateTime.Now.AddMinutes(minute))}";
            String SignKey = HMACSHA1(secretKey, QKeyTime);
            String HttpStr = $"{Method.ToString()}\n{Path}\n{HttpParamStr.ToString()}\n{HeaderStr.ToString()}\n";
            String SignStr = $"sha1\n{QKeyTime}\n{SHA1HASH(HttpStr)}\n";
            String Sign = HMACSHA1(SignKey, SignStr);

            return $"q-sign-algorithm=sha1&q-ak={secretId}&q-sign-time={QKeyTime}&q-key-time={QKeyTime}&q-header-list={String.Join(";", Headers.Keys)}&q-url-param-list={String.Join(";", HttpParam.Keys)}&q-signature={Sign}";
        }

        /// <summary>
        /// 获取请求Host
        /// </summary>
        /// <param name="conf"></param>
        /// <returns></returns>
        private static String GetHost(Models.Config conf)
        {
            return $"{conf.Bucket}.cos.{conf.Region}.myqcloud.com";
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        private static Int64 GetTimeStamp(DateTime dt)
        {
            TimeSpan ts = dt - new DateTime(1970, 1, 1, 8, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        /// <summary>
        /// HMACSHA1加密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private static String HMACSHA1(String key, String body)
        {
            byte[] btkey = Encoding.UTF8.GetBytes(key);
            byte[] btbody = Encoding.UTF8.GetBytes(body);
            using (HMACSHA1 hmac = new HMACSHA1(btkey))
            {
                return String.Join("", hmac.ComputeHash(btbody).ToList().Select(b => b.ToString("x2")).ToArray());
            }
        }

        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        private static String SHA1HASH(String body)
        {
            byte[] btbody = Encoding.UTF8.GetBytes(body);
            using (SHA1 sha1 = new SHA1CryptoServiceProvider())
            {
                return String.Join("", sha1.ComputeHash(btbody).ToList().Select(b => b.ToString("x2")).ToArray());
            }
        }
        #endregion

    }
}
