using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CSRedis;
using System.Threading.Tasks;

namespace XUnitTest
{
    public class CommServiceProvider
    {
        private IServiceProvider ServiceProvider;
        private IConfiguration Configuration;
        public IConfiguration GetConfiguration()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath("/Users/topbrids/Desktop/lfex/tasklfex/Yoyo/lfextask")
                .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            return Configuration;
        }

        public IServiceProvider GetServiceProvider()
        {
            GetConfiguration();

            IServiceCollection services = new ServiceCollection();

            #region 数据库注入
            services.AddDbContext<Yoyo.Entity.SqlContext>((serviceProvider, option) =>
            {
                option.UseMySql(Configuration.GetConnectionString("yoyoServiceConStr"), myop =>
                {
                    myop.ServerVersion(new Version(5, 7, 18), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql)
                        .UnicodeCharSet(Pomelo.EntityFrameworkCore.MySql.Infrastructure.CharSet.Utf8mb4);
                });
            });
            #endregion

            #region 配置注入
            services.Configure<List<Yoyo.IServices.Utils.TaskSettings>>(Configuration.GetSection("TaskSettings"));   //任务配置
            services.Configure<Yoyo.IServices.Utils.ClickCandyP>(Configuration.GetSection("ClickCandyP"));    //广告信息注入
            services.Configure<List<Yoyo.IServices.Utils.SystemUserLevel>>(Configuration.GetSection("SystemUserLevel"));    //广告信息注入
            #endregion

            #region 微信公众号
            // IConfigurationSection WeChatConf = Configuration.GetSection("WeChatConf");
            // services.Configure<Yoyo.Plugin.WeChat.Models.Config>(WeChatConf);
            // Yoyo.Plugin.WeChat.Models.Config WeChatConfig = WeChatConf.Get<Yoyo.Plugin.WeChat.Models.Config>();
            // services.AddHttpClient(WeChatConfig.ClientName);
            // services.AddScoped<Yoyo.IPlugins.IWeChatPlugin, Yoyo.Plugin.WeChat.WeChatPlugin>();
            #endregion

            #region 支付宝插件
            // IConfigurationSection AliConfigJson = Configuration.GetSection("AliPayConfig");
            // Yoyo.Plugin.Alipay.Models.Config AliConfig = AliConfigJson.Get<Yoyo.Plugin.Alipay.Models.Config>();
            // services.Configure<Yoyo.Plugin.Alipay.Models.Config>(AliConfigJson);
            // services.AddHttpClient(AliConfig.ClientName);
            // services.AddScoped<Yoyo.IPlugins.IAlipayPlugin, Yoyo.Plugin.Alipay.Payment>();
            #endregion

            #region 缓存注入
            services.AddSingleton<CSRedisClient>(o => new CSRedisClient(Configuration.GetConnectionString("CacheConnection")));
            #endregion

            #region 接口注入
            services.AddSingleton<Yoyo.IServices.IMember.ISubscribe, Yoyo.Service.Member.Subscribe>(); //Redis通讯接口 --使用单例注入
            services.AddScoped<Yoyo.IServices.IMember.ITeams, Yoyo.Service.Member.Teams>();   //团队接口
            #endregion

            #region 定时任务注入
            // IConfigurationSection JobsDetail = Configuration.GetSection("Jobs");
            // services.Configure<List<Yoyo.Core.JobDetail>>(JobsDetail);
            // services.AddSingleton<Yoyo.Core.JobScheduler>();
            // //==============================任务组==============================//
            // services.AddTransient<Yoyo.Jobs.DailyCloseTask>();           //每日关闭过期任务
            // services.AddTransient<Yoyo.Jobs.DailyUpdateDividend>();      //更新星级-大区-小区
            // services.AddTransient<Yoyo.Jobs.DealWithTradeOrder>();       //定时关闭超时任务
            // services.AddTransient<Yoyo.Jobs.UpdateTeamStar>();           //定时更新星级大区和小区
            // services.AddTransient<Yoyo.Jobs.DailyInviteRanking>();       //每日邀请排行榜分红
            // services.AddTransient<Yoyo.Jobs.DailyUpdateBon>();           //每日更新邀请排行榜加成
            // //==============================任务组==============================//
            // services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            // services.AddSingleton<IJobFactory, Yoyo.Core.JobFactoryService>();
            #endregion

            //缓存注入
            services.AddMemoryCache();
            //HttpClientFactory注入
            services.AddHttpClient();

            ServiceProvider = services.BuildServiceProvider();

            #region 启动消息订阅
            Yoyo.IServices.IMember.ISubscribe syncTeams = ServiceProvider.GetRequiredService<Yoyo.IServices.IMember.ISubscribe>();
            CSRedisClient.SubscribeObject subScribe = null;
            CSRedisClient redis = ServiceProvider.GetRequiredService<CSRedisClient>();
            subScribe = redis.Subscribe(
               ("YoYo_Member_Regist", async msg => await syncTeams.SubscribeMemberRegist(msg.Body)),
               ("YoYo_Member_Certified", async msg => await syncTeams.SubscribeMemberCertified(msg.Body)),
               ("Lfex_Member_LFChange", async msg => await syncTeams.SubLfChange(msg.Body))
            //    ("YoYo_Member_AD_Share", async msg => await syncTeams.SubscribeClickAd(msg.Body)),
            //    ("YoYo_Member_DoSysTask", async msg => await syncTeams.SubscribeTask(msg.Body)),
            //    ("YoYo_Trade_SysBuyer", async msg => await syncTeams.SubscribeTradeSystem(msg.Body))
            );
            #endregion

            #region 定时任务启动
            // Yoyo.Core.JobScheduler jobInit = ServiceProvider.GetRequiredService<Yoyo.Core.JobScheduler>();
            // async Task jobStart()
            // {
            //     await jobInit.Start();
            //     //==============================任务==============================//
            //     // await jobInit.AddTask<Yoyo.Jobs.DailyCloseTask>();           //每日关闭过期任务
            //     // await jobInit.AddTask<Yoyo.Jobs.DailyUpdateDividend>();      //更新星级-大区-小区
            //     // await jobInit.AddTask<Yoyo.Jobs.DealWithTradeOrder>();       //定时关闭超时订单
            //     // await jobInit.AddTask<Yoyo.Jobs.UpdateTeamStar>();           //定时更新星级大区和小区
            //     // await jobInit.AddTask<Yoyo.Jobs.DailyInviteRanking>();       //每日邀请排行榜分红
            //     // await jobInit.AddTask<Yoyo.Jobs.DailyUpdateBon>();           //每日更新邀请排行榜加成
            //     //==============================任务==============================//
            // }
            // jobStart().Wait();
            #endregion

            return ServiceProvider;
        }
    }
}
