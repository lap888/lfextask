using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Yoyo.IPlugins.Utils;

namespace Yoyo.Plugin.WeChatPay
{
    /// <summary>
    /// 微信支付
    /// </summary>
    public class Payment : IPlugins.IWeChatPay
    {
        private readonly HttpClient ClientPay;
        private readonly HttpClient RefundClient;
        private readonly Models.Config config;

        public Payment(IHttpClientFactory factory, IOptionsMonitor<Models.Config> monitor)
        {
            config = monitor.CurrentValue;
            ClientPay = factory.CreateClient(config.ClientPay);
            RefundClient = factory.CreateClient(config.ClientRefund);
        }

        /// <summary>
        /// 请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<T> Execute<T>(IWePayRequest<T> request) where T : WePayResponse, new()
        {
            WeXmlDoc xml = request.GetXmlDoc();
            xml.Add("sign", xml.GetSign(config.ApiV3Key));
            String Body = xml.ToXmlStr();
            String ResultStr = String.Empty;
            T Result = new T();
            try
            {
                StringContent Content = new StringContent(Body, Encoding.UTF8);
                HttpResponseMessage Context = await this.ClientPay.PostAsync(request.GetUrl(), Content);
                ResultStr = await Context.Content.ReadAsStringAsync();
                Content.Dispose();
                Context.Dispose();
                using (var reader = new StringReader(ResultStr))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    Result = (T)serializer.Deserialize(reader);
                    Result.Content = ResultStr;
                    return Result;
                }
            }
            catch (Exception ex)
            {
                Result.Content = ResultStr;
                Result.ErrCode = ex.Source;
                Result.ErrCodeDesc = ex.Message;
                return Result;
            }
        }
    }
}
