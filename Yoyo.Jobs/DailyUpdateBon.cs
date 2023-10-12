using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace Yoyo.Jobs
{
    public class DailyUpdateBon : IJob
    {
        private readonly IServiceProvider ServiceProvider;

        public DailyUpdateBon(IServiceProvider service)
        {
            this.ServiceProvider = service;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            //停止任务运行
            // return;
            using (var service = this.ServiceProvider.CreateScope())
            {
                try
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    Entity.SqlContext SqlContext = service.ServiceProvider.GetRequiredService<Entity.SqlContext>();
                    CSRedis.CSRedisClient RedisCache = service.ServiceProvider.GetRequiredService<CSRedis.CSRedisClient>();

                    List<long> UserIds = (await SqlContext.Dapper.QueryAsync<long>("SELECT u.id FROM (SELECT * FROM yoyo_member_invite_ranking WHERE  Phase = DATE_FORMAT(NOW(), '%m') AND InviteTotal >= 100 ORDER BY InviteTotal DESC LIMIT 50) AS rank INNER JOIN `user` AS u ON rank.UserId = u.id LIMIT 50")).ToList();
                    RedisCache.Del("UserBon");
                    foreach (var item in UserIds)
                    {
                        var Index = UserIds.FindIndex(o => o == item) + 1;
                        if (Index == 0) { continue; }
                        Decimal BonRate = 1.00M;
                        if (Index == 1) { BonRate = 2.00M; }
                        if (Index == 2 || Index == 3) { BonRate = 1.80M; }
                        if (Index >= 4 && Index <= 10) { BonRate = 1.50M; }
                        if (Index >= 11 && Index <= 20) { BonRate = 1.30M; }
                        if (Index >= 21 && Index <= 50) { BonRate = 1.10M; }
                        RedisCache.HSet("UserBon", item.ToString(), BonRate);
                    }
                    stopwatch.Stop();
                    Core.SystemLog.Jobs($"每日更新邀请排行榜加成 执行完成,执行时间:{stopwatch.Elapsed.TotalSeconds}秒");
                }
                catch (Exception ex)
                {
                    Core.SystemLog.Jobs("每日更新邀请排行榜加成 发生错误", ex);
                }
            }
        }
    }
}
