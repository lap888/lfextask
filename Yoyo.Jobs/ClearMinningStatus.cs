using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Yoyo.Jobs
{
    public class ClearMinningStatus : IJob
    {
        private readonly IServiceProvider ServiceProvider;
        public ClearMinningStatus(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            using (var service = this.ServiceProvider.CreateScope())
            {
                try
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    Entity.SqlContext SqlContext = service.ServiceProvider.GetRequiredService<Entity.SqlContext>();

                    Entity.SqlContextYoyo SqlContextYoyo = service.ServiceProvider.GetRequiredService<Entity.SqlContextYoyo>();

                    //未挖矿矿机ID
                    await SqlContext.Dapper.ExecuteAsync("update `minnings` set status = 0 where minningId=0 and id in (select id from (select a.id from (select id, userId from `minnings` where id not in (select DISTINCT `MId` from `yoyo_task_record` where DATE_SUB(CURDATE(), INTERVAL 3 DAY)<= date(`StartTime`))) a left join `user` u on a.userId = u.id where u.`auditState`= 2 and DATE_SUB(CURDATE(), INTERVAL 3 DAY)>=date(`utime`)) tmp)");
                    //定时清
                    await SqlContext.Dapper.ExecuteAsync("update `minnings` set `minningStatus`=0,`workingTime`=NULL,`workingEndTime`=NULL,`updatedAt`=now()");

                    //清哟哟吧2.0矿机
                    await SqlContextYoyo.Dapper.ExecuteAsync("update `s_minnings` set `minningStatus`=0,`workingTime`=NULL,`workingEndTime`=NULL,`updatedAt`=now()");

                    //数据统计 
                    //交易所数量
                    var exChangeCount = await SqlContext.Dapper.QueryFirstOrDefaultAsync<int>("select sum(`Balance`) CandyTotal from `user_account_wallet` where CoinType='糖果'");
                    //当前用户糖
                    var candyNowCount = await SqlContextYoyo.Dapper.QueryFirstOrDefaultAsync<int>("select sum(candyNum) from user");
                    //预计产出糖
                    var willCandyCount = await SqlContextYoyo.Dapper.QueryFirstOrDefaultAsync<int>("SELECT IFNULL(sum(prePro),0) AS totalPro FROM ( SELECT timeC * product AS prePro FROM ( SELECT DATEDIFF(endTime, now()) AS timeC, minningId , CASE minningId WHEN 1 THEN 2.3 WHEN 2 THEN 24 WHEN 3 THEN 75 WHEN 4 THEN 460 WHEN 5 THEN 182 WHEN 6 THEN 0.5 WHEN 7 THEN 1.1 WHEN 8 THEN 1400 WHEN 9 THEN 4733 WHEN 10 THEN 11.6 WHEN 11 THEN 2 WHEN 12 THEN 5 WHEN 13 THEN 15 WHEN 14 THEN 45 WHEN 16 THEN 0.4 WHEN 17 THEN 0.4 WHEN 18 THEN 10 WHEN 19 THEN 2 WHEN 20 THEN 1 WHEN 50 THEN 0.4 WHEN 51 THEN 4 WHEN 52 THEN 16.6 WHEN 53 THEN 42 WHEN 54 THEN 166 WHEN 55 THEN 460 WHEN 56 THEN 4733 WHEN 57 THEN 1400 WHEN 60 THEN 0.24 WHEN 61 THEN 2.46 WHEN 62 THEN 9.96 WHEN 63 THEN 25.2 WHEN 64 THEN 99.6 WHEN 65 THEN 276 WHEN 67 THEN 840 WHEN 100 THEN 0.4 WHEN 101 THEN 4 WHEN 102 THEN 16.6 WHEN 103 THEN 42 WHEN 104 THEN 166 WHEN 105 THEN 460 WHEN 106 THEN 4733 WHEN 110 THEN 0.24 WHEN 111 THEN 2.46 WHEN 112 THEN 9.96 WHEN 113 THEN 25.2 WHEN 114 THEN 99.6 WHEN 115 THEN 276 WHEN 117 THEN 840 WHEN 120 THEN 1.1 WHEN 127 THEN 700 WHEN 125 THEN 230 WHEN 124 THEN 83 WHEN 123 THEN 21 WHEN 122 THEN 8.3 WHEN 121 THEN 2.05 ELSE 0 END AS product FROM `minnings` WHERE TO_DAYS(now()) <= TO_DAYS(endTime) ) a ) b");

                    var total = exChangeCount + candyNowCount + willCandyCount;
                    var recordSayNow = $"交易所数量={exChangeCount}\r\n当前用户糖={candyNowCount}\r\n预计产出糖={willCandyCount}\r\n总量={total}";
                    System.Console.WriteLine("ok...");
                    stopwatch.Stop();
                    Core.SystemLog.Info(recordSayNow);
                    Core.SystemLog.Jobs($"每日更新矿机状态成功 执行完成,执行时间:{stopwatch.Elapsed.TotalSeconds}秒");
                }
                catch (Exception ex)
                {
                    Core.SystemLog.Jobs("每日更新矿机状态失败 发生错误", ex);
                }
            }
        }
    }
    public class DoMinningsRecord
    {
        public int MId { get; set; }
        public int UserId { get; set; }
    }
}