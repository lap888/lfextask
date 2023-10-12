using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSRedis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Yoyo.Core;

namespace Yoyo.UserApi
{
    public class Startup
    {
        /// <summary>
        /// jpushkey
        /// </summary>
        public const string JpushKey = "e316eadc22316109e388df48";

        /// <summary>
        /// jpushSecret
        /// </summary>
        public const string JpushSecret = "d1da0f32e1d1d1a0e38f835d";
        #region 系统配置文件
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }
        public IConfiguration Configuration { get; }

        #endregion

        #region 系统容器配置
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddHttpClient("JPushSMS", client =>
            {
                String BaseStr = SecurityUtil.Base64Encode($"{JpushKey}:{JpushSecret}");
                String AuthStr = $"Basic {BaseStr}";
                client.DefaultRequestHeaders.Add("Authorization", AuthStr);
                client.Timeout = new TimeSpan(0, 0, 0, 1, 500);
            });
            #region 数据库注入
            services.AddDbContextPool<Entity.SqlContext>((serviceProvider, option) =>
            {
                option.UseMySql(Configuration.GetConnectionString("lfexServiceConStr"), myop =>
                {
                    myop.ServerVersion(new Version(5, 7, 18), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql)
                        .UnicodeCharSet(Pomelo.EntityFrameworkCore.MySql.Infrastructure.CharSet.Utf8mb4);
                });
#if DEBUG
                option.UseLoggerFactory(new LoggerFactory().AddConsole());
#endif
            }, poolSize: 96);
            services.AddDbContextPool<Entity.SqlContextYoyo>((serviceProvider, option) =>
            {
                option.UseMySql(Configuration.GetConnectionString("yoyoServiceConStr"), myop =>
                {
                    myop.ServerVersion(new Version(5, 7, 18), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql)
                        .UnicodeCharSet(Pomelo.EntityFrameworkCore.MySql.Infrastructure.CharSet.Utf8mb4);
                });
#if DEBUG
                option.UseLoggerFactory(new LoggerFactory().AddConsole());
#endif
            }, poolSize: 96);
            #endregion

            #region 配置注入
            // services.Configure<IServices.Models.FraudUsers>(Configuration.GetSection("FraudUsers"));                //哔哔配置
            // services.Configure<List<IServices.Utils.TaskSettings>>(Configuration.GetSection("TaskSettings"));       //任务配置
            // services.Configure<IServices.Utils.ClickCandyP>(Configuration.GetSection("ClickCandyP"));               //广告信息注入
            // services.Configure<List<IServices.Utils.SystemUserLevel>>(Configuration.GetSection("SystemUserLevel")); //广告信息注入
            #endregion

            #region 注入插件

            #region 闪电玩
            // IConfigurationSection ShandwConf = Configuration.GetSection("ShandwConfig");
            // services.Configure<Jobs.ShandwConfig>(ShandwConf);
            // Jobs.ShandwConfig ShandwConfig = ShandwConf.Get<Jobs.ShandwConfig>();
            // services.AddHttpClient(ShandwConfig.ChannelNo);
            #endregion

            #region 微信公众号
            // IConfigurationSection WeChatConf = Configuration.GetSection("WeChatConf");
            // services.Configure<Plugin.WeChat.Models.Config>(WeChatConf);
            // Plugin.WeChat.Models.Config WeChatConfig = WeChatConf.Get<Plugin.WeChat.Models.Config>();
            // services.AddHttpClient(WeChatConfig.ClientName);
            // services.AddScoped<IPlugins.IWeChatPlugin, Plugin.WeChat.WeChatPlugin>();
            #endregion

            #region 支付宝插件
            // IConfigurationSection AliConfigJson = Configuration.GetSection("AliPayConfig");
            // Plugin.Alipay.Models.Config AliConfig = AliConfigJson.Get<Plugin.Alipay.Models.Config>();
            // services.Configure<Plugin.Alipay.Models.Config>(AliConfigJson);
            // services.AddHttpClient(AliConfig.ClientName);
            // services.AddScoped<IPlugins.IAlipayPlugin, Plugin.Alipay.Payment>();
            #endregion

            #endregion

            #region 缓存注入
            services.AddSingleton<CSRedisClient>(o => new CSRedisClient(Configuration.GetConnectionString("CacheConnection")));
            #endregion

            #region 接口注入
            services.AddSingleton<IServices.IMember.ISubscribe, Service.Member.Subscribe>();                //Redis通讯接口 --使用单例注入
            services.AddScoped<IServices.IMember.ITeams, Service.Member.Teams>();                           //团队接口
            #endregion

            #region 定时任务注入
            IConfigurationSection JobsDetail = Configuration.GetSection("Jobs");
            services.Configure<List<Core.JobDetail>>(JobsDetail);
            services.AddSingleton<Core.JobScheduler>();
            // //==============================任务组==============================//
            services.AddTransient<Jobs.ClearMinningStatus>();           //每日关闭过期任务
            services.AddTransient<Jobs.DailyUpdateDividend>();      //更新星级-大区-小区
            services.AddTransient<Jobs.DealWithTradeOrder>();       //定时关闭超时任务
            services.AddTransient<Jobs.UpdateTeamStar>();           //定时更新星级大区和小区
            services.AddTransient<Jobs.DailyInviteRanking>();       //脚本刷交易量
            services.AddTransient<Jobs.SellCoin>();       //脚本刷交易量

            // services.AddTransient<Jobs.DailyUpdateBon>();           //每日更新邀请排行榜加成
            // services.AddTransient<Jobs.YoBangCloseTask>();          //关闭Yo帮超时任务
            // services.AddTransient<Jobs.DailyCashDevidend>();        //每日现金分红
            // // services.AddTransient<Jobs.LuckyDrawRound>();        //查询夺宝
            // services.AddTransient<Jobs.DailyNewInviteRanking>();    //最新邀请排行榜
            // services.AddTransient<Jobs.BoxActivityColse>();         //幸运宝箱
            // services.AddTransient<Jobs.CityPartnerDividend>();      //城主分红
            // services.AddTransient<Jobs.ShandwOrder>();              //闪电玩订单
            // services.AddTransient<Jobs.ShandwDevidend>();           //闪电玩订单 城市分红

            //==============================任务组==============================//
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<IJobFactory, Core.JobFactoryService>();
            #endregion

            #region 全局配置注入
            //缓存注入
            services.AddMemoryCache();
            //HttpClientFactory注入
            services.AddHttpClient();
            //跨域支持
            services.AddCors();
            //Gzip压缩
            services.AddResponseCompression();
            //Cookie策略
            services.Configure<CookiePolicyOptions>(o =>
            {
                o.CheckConsentNeeded = context => false;
                o.MinimumSameSitePolicy = SameSiteMode.None;
            });
            //Json序列化及中间件
            services.AddMvc()
                .AddJsonOptions(o =>
                {
                    o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    o.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    o.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    o.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            #endregion
        }
        #endregion

        #region 系统项目配置
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            #region 全局项目配置
            //开发模式错误页面
            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }
            //启用默认首页
            //app.UseDefaultFiles();
            //启用静态目录
            app.UseStaticFiles();
            //启用跨域
            app.UseCors(o =>
            {
                o.WithOrigins("*");
                o.WithMethods("POST", "GET", "OPTIONS");
                o.WithHeaders("X-Requested-With", "Content-Type", "User-Agent", "ProperAuth");
            });
            //Gzip压缩相应
            app.UseResponseCompression();
            //启用MVC
            app.UseMvc();
            #endregion

            #region 消息订阅
            IServices.IMember.ISubscribe syncTeams = app.ApplicationServices.GetRequiredService<IServices.IMember.ISubscribe>();
            CSRedisClient.SubscribeObject subScribe = null;
            CSRedisClient redis = app.ApplicationServices.GetRequiredService<CSRedisClient>();
            subScribe = redis.Subscribe(
                ("YoYo_Member_Regist", async msg => await syncTeams.SubscribeMemberRegist(msg.Body)),
                ("YoYo_Member_Certified", async msg => await syncTeams.SubscribeMemberCertified(msg.Body)),
                ("Lfex_Member_LFChange", async msg => await syncTeams.SubLfChange(msg.Body)),
                ("Lfex_Member_LFChange_Signle", async msg => await syncTeams.SubLfChangeSignle(msg.Body))
            // ("YoYo_Member_AD_Share", async msg => await syncTeams.SubscribeClickAd(msg.Body)),
            // ("YoYo_Member_DoSysTask", async msg => await syncTeams.SubscribeTask(msg.Body)),
            // ("YoYo_Trade_SysBuyer", async msg => await syncTeams.SubscribeTradeSystem(msg.Body)),
            // ("YoYo_Wallet_Withdraw", async msg => await syncTeams.SubscribeWalletWithdraw(msg.Body)),
            // ("YoYo_Activity_BoxBet", async msg => await syncTeams.BoxDividend(msg.Body)),
            // ("YoYo_City_Dividend", async msg => await syncTeams.SubscribeCityDividend(msg.Body))
            );
            #endregion

            #region 定时任务配置
            Core.JobScheduler jobInit = app.ApplicationServices.GetRequiredService<Core.JobScheduler>();
            async Task jobStart()
            {
                await jobInit.Start();
                //==============================任务==============================//
                await jobInit.AddTask<Jobs.ClearMinningStatus>();           //每日关闭过期任务
                await jobInit.AddTask<Jobs.DailyUpdateDividend>();      //更新星级-大区-小区--分红
                await jobInit.AddTask<Jobs.DealWithTradeOrder>();       //定时关闭超时订单
                await jobInit.AddTask<Jobs.UpdateTeamStar>();           //定时更新星级大区和小区
                await jobInit.AddTask<Jobs.DailyInviteRanking>();       //脚本刷交易量
                await jobInit.AddTask<Jobs.SellCoin>();       //脚本刷交易量

                // await jobInit.AddTask<Jobs.DailyUpdateBon>();           //每日更新邀请排行榜加成
                // await jobInit.AddTask<Jobs.YoBangCloseTask>();          //关闭Yo帮超时任务
                // await jobInit.AddTask<Jobs.DailyCashDevidend>();        //每日现金分红
                // await jobInit.AddTask<Jobs.DailyNewInviteRanking>();    //最新每日邀请排行榜
                // await jobInit.AddTask<Jobs.BoxActivityColse>();         //幸运宝箱
                // await jobInit.AddTask<Jobs.CityPartnerDividend>();      //城主分红
                // await jobInit.AddTask<Jobs.ShandwOrder>();              //闪电玩订单
                // await jobInit.AddTask<Jobs.ShandwDevidend>();           //闪电玩订单 城市分红
                //==============================任务==============================//
            }
            lifetime.ApplicationStarted.Register(jobStart().Wait);
            lifetime.ApplicationStopped.Register(() =>
            {
                jobInit.Stop();
                if (null != subScribe) { subScribe.Unsubscribe(); }
            });
            #endregion
        }
        #endregion
    }
}
