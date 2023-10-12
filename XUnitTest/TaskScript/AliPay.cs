using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest
{
    public class AliPay_Test
    {
        private readonly IServiceProvider ServiceProvider;
        public readonly IConfiguration Configuration;
        private readonly Yoyo.Plugin.Alipay.Models.Config AliConfig;

        public AliPay_Test()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath("D:\\Project\\yoyo\\Yoyo\\Yoyo.UserApi\\")
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            IServiceCollection services = new ServiceCollection();
            IConfigurationSection AliConfigJson = Configuration.GetSection("AliPayConfig");
            AliConfig = AliConfigJson.Get<Yoyo.Plugin.Alipay.Models.Config>();
            services.Configure<Yoyo.Plugin.Alipay.Models.Config>(AliConfigJson);
            services.AddHttpClient(AliConfig.ClientName);
            services.AddSingleton<Yoyo.IPlugins.IAlipayPlugin,Yoyo.Plugin.Alipay.Payment>();

            this.ServiceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async Task Test()
        {
            Yoyo.IPlugins.IAlipayPlugin Ali = this.ServiceProvider.GetService<Yoyo.IPlugins.IAlipayPlugin>();

            Yoyo.IPlugins.Request.ReqAlipayTransfer req = new Yoyo.IPlugins.Request.ReqAlipayTransfer {
                OutBizNo = Guid.NewGuid().ToString("N"),
                ProductCode = "TRANS_ACCOUNT_NO_PWD",
                BizScene = "DIRECT_TRANSFER",
                IdentityType = "ALIPAY_USER_ID",
                OrderTitle = "账号测试",
                Identity = "2088002228207553",
                TransAmount = 0.1M
            };

            var zzz = await Ali.Execute(req);

            return;
        }
    }
}
