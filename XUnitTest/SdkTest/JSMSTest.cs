using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Yoyo.IPlugins;
using Yoyo.IPlugins.Request;
using Yoyo.IPlugins.Response;
using Yoyo.Plugin.JSMS;
using Yoyo.Plugin.JSMS.Models;

namespace XUnitTest
{
    public class JSMSTest
    {
        private readonly IServiceProvider ServiceProvider;
        public JSMSTest()
        {
            JSMSConfig config = new JSMSConfig()
            {
                ClientName = "JSMSClient",
                ApiUrl = "https://api.sms.jpush.cn/v1/",
                AppKey = "623cebd8d3b4b9e057b427ac",
                AppSecret = "240a2e8115d5cc37f321a68e"
            };
            IServiceCollection services = new ServiceCollection();
            services.AddHttpClient(config.ClientName,client=> {
                client.BaseAddress = new Uri(config.ApiUrl);
                string AuthStr = config.AppKey + ":" + config.AppSecret;
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(AuthStr)));
            });
            services.AddScoped<ISMSPlugin, JSMSPlugin>();
            services.AddOptions();
            services.Configure<JSMSConfig>(model=> { model = config; });

            ServiceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async void SendSMS()
        {
            var conf = ServiceProvider.GetService<IOptions<JSMSConfig>>();

            var SmsSub = ServiceProvider.GetRequiredService<ISMSPlugin>();

            //var Result = await SmsSub.Execute(new ReqSmsSend() { Mobile = "13663112276", TempId = "1" });

            var Check = await SmsSub.Execute(new ReqSmsVerify("245028878385153") { Code = "21920" });

            return;
        }

    }
}
