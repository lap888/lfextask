using Dapper;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Text;
using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Yoyo.IServices.Response;
using System.Diagnostics;
using Newtonsoft.Json;
using CSRedis;

namespace XUnitTest
{
    public class SdwGame
    {
        private readonly IServiceProvider ServiceProvider;
        private readonly Yoyo.Entity.SqlContext SqlContext;
        public SdwGame()
        {
            var SqlConn = "server=39.100.98.114;port=3306;user id=guo;password=Yaya123...;database=yoyo_service_beta;Charset=utf8mb4;persistsecurityinfo=True;TreatTinyAsBoolean=true";
            var RedisConn = "62.234.70.166:8082,password=870913,defaultDatabase=2,prefix=Y_";

            IServiceCollection services = new ServiceCollection();
            #region 数据库注入
            services.AddDbContextPool<Yoyo.Entity.SqlContext>((serviceProvider, option) =>
            {
                option.UseMySql(SqlConn, myop =>
                {
                    myop.ServerVersion(new Version(5, 7, 28), Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql)
                        .UnicodeCharSet(Pomelo.EntityFrameworkCore.MySql.Infrastructure.CharSet.Utf8mb4);
                });
            }, poolSize: 96);
            #endregion
            services.AddSingleton<CSRedisClient>(o => new CSRedisClient(RedisConn));
            ServiceProvider = services.BuildServiceProvider();
            SqlContext = this.ServiceProvider.GetService<Yoyo.Entity.SqlContext>();
        }

        [Fact]
        public void GameTest()
        {
            var userId = "1111";
            // var sdwId = "2022011855";
            var AuthUrl = "http://www.shandw.com/auth/";

            SortedDictionary<string, string> dict = new SortedDictionary<string, string>();
            DateTimeOffset dto = new DateTimeOffset(DateTime.Now);
            var timeSpan = dto.ToUnixTimeSeconds().ToString().Substring(0, 10);
            var avatarFull = "https://file.yoyoba.cn/" + "images/avatar/default/1.png";
            var sign = SdwGenSign("0f68f74ef06946edad8f8f3dd1bce9c7", "12872", userId, userId, avatarFull, timeSpan,"13800138000");
            dict.Add("channel", "12872");
            dict.Add("openid", userId);
            dict.Add("nick", userId);
            dict.Add("avatar", System.Net.WebUtility.UrlEncode(avatarFull));
            dict.Add("sex", "0");
            dict.Add("phone", "13800138000");
            dict.Add("time", timeSpan);
            dict.Add("sign", sign);
            //dict.Add("gid", sdwId);

            dict.Add("sdw_simple", "4");
            dict.Add("sdw_tt", "1");
            dict.Add("sdw_ld", "1");
            dict.Add("sdw_tl", "1");
            dict.Add("sdw_kf", "1");
            dict.Add("sdw_dl", "1");
            dict.Add("sdw_qd", "1");

            var content = string.Join("&", dict.Where(t => !string.IsNullOrEmpty(t.Value))
                  .Select(t => $"{t.Key}={t.Value}"));
            var Data = AuthUrl + '?' + content;
            return;
        }

        public static string SdwGenSign(string key, string channel, string nick, string uid, string avatar, string time,string phone)
        {
            var StringA = $"channel={channel}&openid={uid}&time={time}&nick={nick}&avatar={avatar}&sex=0&phone={phone}";
            var StringB = StringA + key;
            var sign = MD5(StringB).ToLower();
            return sign;
        }

        public static string MD5(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "Error";
            }
            var md5 = System.Security.Cryptography.MD5.Create();
            string a = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(str)));
            a = a.Replace("-", "");
            return a;
        }
    }
}
