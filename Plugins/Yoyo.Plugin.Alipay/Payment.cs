using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Yoyo.IPlugins.Utils;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Cryptography;
using System.Net.Http.Headers;
using System.Web;
using System.Collections;

namespace Yoyo.Plugin.Alipay
{
    /// <summary>
    /// 支付宝支付
    /// </summary>
    public class Payment : IPlugins.IAlipayPlugin
    {
        private readonly Models.Config Config;
        private readonly HttpClient ClientPay;
        private readonly JsonSerializerSettings JsonSetting;
        public Payment(IHttpClientFactory factory, IOptionsMonitor<Models.Config> monitor)
        {
            this.Config = monitor.CurrentValue;
            ClientPay = factory.CreateClient(Config.ClientName);
            JsonSetting = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None,
            };
        }

        /// <summary>
        /// 请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<AlipayResult<T>> Execute<T>(IAlipayRequest<AlipayResult<T>> request) where T : AlipayResponse, new()
        {
            String ResultStr = String.Empty;
            AlipayResult<T> Result = new AlipayResult<T>();
            try
            {
                String Body = await GetSignStr(request);
                StringContent Content = new StringContent(Body, Encoding.UTF8, "application/x-www-form-urlencoded");
                HttpResponseMessage Context = await this.ClientPay.PostAsync(this.Config.ApiUrl, Content);
                ResultStr = await Context.Content.ReadAsStringAsync();
                Content.Dispose();
                Context.Dispose();

                String ResultData = String.Empty;
                if (SyncVerify(ResultStr, request.GetApiName(), this.Config.PublicKey, out ResultData))
                {
                    Result.Result = JsonConvert.DeserializeObject<T>(ResultData);
                    Result.Content = ResultStr;
                    Result.IsError = !Result.Result.Code.Equals("10000");
                    Result.ErrCode = Result.Result.Code;
                    Result.ErrMsg = Result.Result.Msg;
                    return Result;
                }
                Result.Content = ResultStr;
                Result.ErrCode = "Signature Fail";
                Result.ErrMsg = "验签失败";
                return Result;
            }
            catch (Exception ex)
            {
                Result.Content = ResultStr;
                Result.ErrCode = ex.Source;
                Result.ErrMsg = ex.Message;
                return Result;
            }
        }

        /// <summary>
        /// 获取签名串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<String> GetSignStr<T>(IAlipayRequest<AlipayResult<T>> request) where T : AlipayResponse, new()
        {
            return await Task.Run(() =>
            {

                UtilDictionary PublicParam = new UtilDictionary();
                if (!String.IsNullOrEmpty(this.Config.AppCertSN) && !String.IsNullOrEmpty(this.Config.AlipayCertSN))
                {
                    PublicParam.Add("alipay_root_cert_sn", this.Config.AlipayCertSN);
                    PublicParam.Add("app_cert_sn", this.Config.AppCertSN);
                }
                PublicParam.Add("app_id", this.Config.AppId);
                PublicParam.Add("format", "JSON");
                PublicParam.Add("charset", "utf-8");
                PublicParam.Add("sign_type", "RSA2");
                PublicParam.Add("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                PublicParam.AddAll(request.GetPublicParam());
                PublicParam.Add("biz_content", JsonConvert.SerializeObject(request.GetParam(), JsonSetting));
                PublicParam.Add("sign", Signature(PublicParam, this.Config.PrivateKey));

                StringBuilder Body = new StringBuilder();
                PublicParam.Aggregate(Body, (s, i) => s.Append($"{i.Key}={HttpUtility.UrlEncode(i.Value, Encoding.UTF8)}&"));
                Body.Remove(Body.Length - 1, 1);

                return Body.ToString();
            });
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="PublicParam"></param>
        /// <param name="PrivateKey"></param>
        /// <returns></returns>
        private static String Signature(Dictionary<String, String> PublicParam, String PrivateKey)
        {
            SortedDictionary<String, String> SortedParam = new SortedDictionary<String, String>(PublicParam, StringComparer.Ordinal);
            StringBuilder SignStr = new StringBuilder();
            SortedParam.Aggregate(SignStr, (s, i) => s.Append($"{i.Key}={i.Value}&"));
            SignStr.Remove(SignStr.Length - 1, 1);
            RSA rsa = Security.CreateRsaProviderFromPrivateKey(PrivateKey);
            return rsa.SignData(SignStr.ToString(), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="ResultStr"></param>
        /// <param name="ApiName"></param>
        /// <param name="PublicKey"></param>
        /// <param name="ResultData"></param>
        /// <returns></returns>
        private static Boolean SyncVerify(String ResultStr, String ApiName, String PublicKey, out String ResultData)
        {
            IDictionary Pairs = JsonConvert.DeserializeObject<IDictionary>(ResultStr);
            String ResultKey = ApiName.Replace(".", "_") + "_response";
            ResultData = JsonConvert.SerializeObject(Pairs[ResultKey], Formatting.None);
            String Sign = (String)Pairs["sign"];

            RSA rsa = Security.CreateRsaProviderFromPublicKey(PublicKey);
            return rsa.VerifySign(ResultData, Sign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

    }
}
