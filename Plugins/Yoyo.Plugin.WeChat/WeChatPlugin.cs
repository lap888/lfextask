using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Yoyo.IPlugins;

namespace Yoyo.Plugin.WeChat
{
    /// <summary>
    /// 微信公众号
    /// </summary>
    public class WeChatPlugin : IWeChatPlugin
    {
        private readonly Models.Config config;
        private readonly HttpClient client;
        public WeChatPlugin(IHttpClientFactory httpfactory, IOptionsMonitor<Models.Config> monitor)
        {
            config = monitor.CurrentValue;
            client = httpfactory.CreateClient(config.ClientName);
        }

        /// <summary>
        /// Code换OpenId
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public async Task<String> GetOpenId(string Code)
        {
            String url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code";
            try
            {
                String res = await client.GetStringAsync(String.Format(url, config.AppId, config.Secret, Code));
                Dictionary<string, object> dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
                Object OpenId = null;
                if (dic.TryGetValue("openid", out OpenId))
                {
                    return OpenId.ToString();
                }
                return String.Empty;
            }
            catch
            {
                return String.Empty;
            }
        }
    }
}
