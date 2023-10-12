using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;
using System.Security.Cryptography;
using Dapper;

namespace Yoyo.Jobs
{
    /// <summary>
    /// 闪电玩订单
    /// </summary>
    public class ShandwOrder : IJob
    {
        private readonly ShandwConfig config;
        private readonly IServiceProvider ServiceProvider;

        public ShandwOrder(IServiceProvider service, IOptionsMonitor<ShandwConfig> monitor)
        {
            this.ServiceProvider = service;
            this.config = monitor.CurrentValue;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var service = this.ServiceProvider.CreateScope())
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                try
                {
                    Entity.SqlContext SqlContext = service.ServiceProvider.GetRequiredService<Entity.SqlContext>();
                    CSRedis.CSRedisClient RedisCache = service.ServiceProvider.GetRequiredService<CSRedis.CSRedisClient>();

                    List<ShandwOrderModel> Orders = new List<ShandwOrderModel>();
                    Boolean IsMore = true;
                    Int32 PageIndex = 0;

                    do
                    {
                        var ListData = await GetOrder(PageIndex, 100, 10);
                        PageIndex++;
                        if (ListData == null || ListData.Count < 1) { IsMore = false; }
                        else
                        {
                            Orders.AddRange(ListData);
                        }
                    } while (IsMore);

                    Int32 rows = 0;
                    StringBuilder InsertSql = new StringBuilder();
                    InsertSql.Append("INSERT INTO `yoyo_shandw_order`(`ChannelNo`, `ChannelUid`, `ChannelOrderNo`, `GameAppId`, `Product`, `UserId`, `PayMoney`, `Amount`, `PayTime`, `State`) VALUES ");
                    foreach (var item in Orders)
                    {
                        if (SqlContext.Dapper.QueryFirstOrDefault<Int32>($"SELECT COUNT(Id) FROM yoyo_shandw_order WHERE ChannelOrderNo = '{item.OrderNo}';") > 0)
                        {
                            continue;
                        }
                        InsertSql.Append($"('{item.Channel}', '{item.ChannelUid}', '{item.OrderNo}', '{item.GameAppId}', '{item.Product}', '{item.UserId}', {item.PayMoney * 0.01M}, {Math.Round(item.PayMoney * 0.01M * 0.38M, 4)}, '{StampToDateTime(item.PayTime.ToString())}', 1),");
                        rows++;
                    }
                    if (rows > 0)
                    {
                        InsertSql.Remove(InsertSql.Length - 1, 1);
                        await SqlContext.Dapper.ExecuteAsync(InsertSql.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Yoyo.Core.SystemLog.Debug($"闪电玩订单=>", ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 订单查询
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="Minutes"></param>
        /// <returns></returns>
        private async Task<List<ShandwOrderModel>> GetOrder(Int32 PageIndex, Int32 PageSize = 100, Int32 Minutes = 60)
        {
            using (var service = this.ServiceProvider.CreateScope())
            {
                IHttpClientFactory factory = service.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                HttpClient client = factory.CreateClient(this.config.ChannelNo);

                String Url = "https://h5gm2.shandw.com/open/channel/queryPayByChannel";

                Dictionary<String, String> Pairs = new Dictionary<String, String>();
                Pairs.Add("account", this.config.ChannelAccount);
                Pairs.Add("channel", this.config.ChannelNo);
                Pairs.Add("sec", GetTimeStamp());

                StringBuilder Body = new StringBuilder();
                Pairs.Aggregate(Body, (s, i) => s.Append($"{i.Key}={i.Value}&"));
                Body.Append("key=").Append(this.config.ApiKey);

                Pairs.Add("sign", MD5(Body.ToString()));
                Pairs.Add("page", PageIndex.ToString());
                Pairs.Add("pageSize", PageSize.ToString());
                Pairs.Add("stime", GetTimeStamp(DateTime.Now.AddMinutes(-Minutes)));
                Pairs.Add("etime", GetTimeStamp());

                StringBuilder Param = new StringBuilder();
                Pairs.Aggregate(Param, (s, i) => s.Append($"{i.Key}={i.Value}&"));
                Param.Remove(Param.Length - 1, 1);

                HttpResponseMessage response = await client.GetAsync($"{Url}?{Param.ToString()}");
                String data = await response.Content.ReadAsStringAsync();
                ShandwResult result = JsonConvert.DeserializeObject<ShandwResult>(data);

                if (result.Code != 1 || result.Data == null) { return null; }

                return result.Data.List;
            }

        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp(DateTime? Dt = null)
        {
            if (Dt == null) { Dt = DateTime.Now; }
            TimeSpan ts = (DateTime)Dt - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        /// <summary>
        /// 时间戳转为时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private DateTime StampToDateTime(string time)
        {
            time = time.Substring(0, 10);
            double timestamp = Convert.ToInt64(time);
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(timestamp).ToLocalTime();
            return dateTime;

        }

        /// <summary>
        /// MD5加密[大写]
        /// </summary>
        /// <param name="value">需要加密的字符串</param>
        /// <param name="IsShort">是否使用16位加密[默认:false]</param>
        /// <returns></returns>
        public static string MD5(string value, bool IsShort = false)
        {
            try
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] bs = Encoding.UTF8.GetBytes(value);
                bs = md5.ComputeHash(bs);
                string CryptoStr;
                if (IsShort)
                {
                    CryptoStr = BitConverter.ToString(bs, 4, 8).Replace("-", "");
                }
                else
                {
                    StringBuilder s = new StringBuilder();
                    foreach (byte b in bs) { s.Append(b.ToString("x2")); }
                    CryptoStr = s.ToString();
                }
                return CryptoStr.ToUpper();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 闪电玩配置
    /// </summary>
    public class ShandwConfig
    {
        /// <summary>
        /// 渠道号
        /// </summary>
        public String ChannelNo { get; set; }

        /// <summary>
        /// 渠道账号
        /// </summary>
        public String ChannelAccount { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        public String Key { get; set; }

        /// <summary>
        /// ApiKey
        /// </summary>
        public String ApiKey { get; set; }
    }

    public class ShandwResult
    {
        public Int32 Code { get; set; }

        public ShandwResultData Data { get; set; }
    }

    public class ShandwResultData
    {
        public List<ShandwOrderModel> List { get; set; }
    }

    /// <summary>
    /// 闪电玩订单
    /// </summary>
    public class ShandwOrderModel
    {
        /// <summary>
        /// CP订单号
        /// </summary>
        [JsonProperty("cpOrderId")]
        public String OrderNo { get; set; }
        /// <summary>
        /// 渠道ID
        /// </summary>
        [JsonProperty("channel")]
        public String Channel { get; set; }
        /// <summary>
        /// 闪电玩用户ID
        /// </summary>
        [JsonProperty("uid")]
        public String ChannelUid { get; set; }
        /// <summary>
        /// 闪电玩应用ID
        /// </summary>
        [JsonProperty("appId")]
        public String GameAppId { get; set; }
        /// <summary>
        /// 产品
        /// </summary>
        [JsonProperty("product")]
        public String Product { get; set; }
        /// <summary>
        /// 会员编号
        /// </summary>
        [JsonProperty("openId")]
        public String UserId { get; set; }
        /// <summary>
        /// 金额（分）
        /// </summary>
        [JsonProperty("money")]
        public Int32 PayMoney { get; set; }
        /// <summary>
        /// 支付时间戳
        /// </summary>
        [JsonProperty("time")]
        public Int64 PayTime { get; set; }
    }
}
