using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Yoyo.IPlugins.Utils;
using Newtonsoft.Json;

namespace Yoyo.Plugin.JSMS
{
    /// <summary>
    /// 极光短信
    /// </summary>
    public class JSMSPlugin : IPlugins.ISMSPlugin
    {
        private readonly HttpClient client;
        private readonly Models.JSMSConfig config;
        private readonly JsonSerializerSettings jsonSettings;
        public JSMSPlugin(IHttpClientFactory factory, IOptionsMonitor<Models.JSMSConfig> monitor)
        {
            config = monitor.CurrentValue;
            jsonSettings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
            client = factory.CreateClient(config.ClientName);
        }
        public async Task<T> Execute<T>(ISMSRequest<T> request) where T : SMSResponse, new()
        {
            String resultStr = string.Empty;
            T ResultData = new T();
            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(request, jsonSettings));
                HttpResponseMessage result = await this.client.PostAsync(request.GetUrl(), content);
                resultStr = await result.Content.ReadAsStringAsync();

                #region 释放资源
                content.Dispose();
                result.Dispose();
                #endregion
                ResultData = JsonConvert.DeserializeObject<T>(resultStr, jsonSettings);
                ResultData.Content = resultStr;
                return ResultData;
            }
            catch (Exception ex)
            {
                ResultData.Content = resultStr;
                ResultData.Error = new ErrorBody() {
                    ErrCode = ex.Source,
                    ErrMsg = ex.Message
                };
                return ResultData;
            }
        }
    }
}
