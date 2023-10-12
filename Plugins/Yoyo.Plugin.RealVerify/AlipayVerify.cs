using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Yoyo.IPlugins.Utils;

namespace Yoyo.Plugin.RealVerify
{
    /// <summary>
    /// 支付实名验证
    /// </summary>
    public class AlipayVerify : IPlugins.IRealVerify
    {
        private readonly HttpClient client;
        private readonly Models.Config config;
        public AlipayVerify(IHttpClientFactory factory, IOptionsMonitor<Models.Config> monitor)
        {
            this.config = monitor.CurrentValue;
            this.client = factory.CreateClient(config.ClientName);
        }

        /// <summary>
        /// 请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<T> Execute<T>(IVerifyRequest<T> request) where T : RealVerifyResponse, new()
        {
            SortedDictionary<String, String> PublicParam = new SortedDictionary<String, String>()
            {
                {"AccessKeyId",config.AccessKey },
                {"Format","JSON" },
                {"Version","2017-03-31" },
                {"SignatureMethod","HMAC-SHA1" },
                {"SignatureVersion","1.0" },
                {"Action","ExecuteRequest" },
                {"Service","fin_face_verify" },
                {"Timestamp",DateTime.UtcNow.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'",CultureInfo.CreateSpecificCulture("en-US"))},
                {"SignatureNonce",Guid.NewGuid().ToString() },
                {"ServiceParameters",JsonConvert.SerializeObject(request.GetPairs()) },
            };
            String Sign = ComputeSign(PublicParam, config.AccessSecret);
            PublicParam.Add("Signature", RFCEncode(Sign));
            String ResultStr = String.Empty;
            T ResultData = new T();
            try
            {
                StringBuilder content = new StringBuilder();
                PublicParam.Aggregate(content, (s, i) => s.Append($"{i.Key}={i.Value}&"));
                content.Remove(content.Length - 1, 1);
                StringContent HttpContent = new StringContent(content.ToString());
                HttpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                HttpResponseMessage HttpResponse = await this.client.PostAsync(this.config.ApiUrl, HttpContent);
                ResultStr = await HttpResponse.Content.ReadAsStringAsync();
                #region 释放资源
                HttpContent.Dispose();
                HttpResponse.Dispose();
                #endregion
                ResultData = JsonConvert.DeserializeObject<T>(ResultStr);
                ResultData.Content = ResultStr;
                return ResultData;
            }
            catch (Exception ex)
            {
                ResultData.Content = ResultStr;
                ResultData.ErrCode = "SDK Exception";
                ResultData.ErrMsg = ex.Message;
                return ResultData;
            }
        }

        #region 私有
        /// <summary>
        /// 计算签名
        /// </summary>
        /// <param name="KeyVal"></param>
        /// <param name="Secret"></param>
        /// <returns></returns>
        private static String ComputeSign(SortedDictionary<String, String> KeyVal, String Secret)
        {
            StringBuilder SignStr = new StringBuilder();
            KeyVal.Aggregate(SignStr, (s, i) => s.Append($"{RFCEncode(i.Key)}={RFCEncode(i.Value)}&"));
            StringBuilder ToSign = new StringBuilder();
            ToSign.Append("POST&");
            ToSign.Append(RFCEncode("/"));
            ToSign.Append("&");
            ToSign.Append(RFCEncode(SignStr.ToString().TrimEnd('&')));
            byte[] HmacKey = Encoding.UTF8.GetBytes(Secret + "&");
            byte[] HmacData = Encoding.UTF8.GetBytes(ToSign.ToString());
            HMACSHA1 hmac = new HMACSHA1(HmacKey);
            byte[] HmacHash = hmac.ComputeHash(HmacData);
            return Convert.ToBase64String(HmacHash);
        }

        private static String RFCEncode(String Str)
        {
            StringBuilder StrBuilder = new StringBuilder();
            String RFC3986Str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            byte[] StrByte = Encoding.UTF8.GetBytes(Str);
            foreach (char c in StrByte)
            {
                if (RFC3986Str.IndexOf(c) >= 0)
                {
                    StrBuilder.Append(c);
                }
                else
                {
                    StrBuilder.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)c));
                }
            }
            return StrBuilder.ToString();
        }
        #endregion

    }
}
