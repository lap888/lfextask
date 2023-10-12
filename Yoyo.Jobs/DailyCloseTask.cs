using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using Yoyo.IServices.Response;
using System.Diagnostics;

namespace Yoyo.Jobs
{
    /// <summary>
    /// 每日关闭任务
    /// </summary>
    public class DailyCloseTask : IJob
    {
        private readonly IServiceProvider ServiceProvider;

        public DailyCloseTask(IServiceProvider service)
        {
            this.ServiceProvider = service;
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
                    //==============================查找任务配置==============================//
                    List<IServices.Utils.TaskSettings> Settings = service.ServiceProvider.GetRequiredService<IOptionsMonitor<List<IServices.Utils.TaskSettings>>>().CurrentValue;
                    //==============================查找任务配置==============================//
                    IServices.IMember.ITeams Team = service.ServiceProvider.GetRequiredService<IServices.IMember.ITeams>();

                    List<UserTaskInfo> Users = (await SqlContext.Dapper.QueryAsync<UserTaskInfo>("SELECT `id` AS `TaskId`,`userId` AS `UserId`,`minningId` AS `TaskLevel` FROM `minnings` WHERE TO_DAYS(Now())>=TO_DAYS(`endTime`) AND `status`=1")).ToList();
                    if (Users.Count > 0)
                    {
                        String TaskIds = String.Join(",", Users.Select(o => o.TaskId).ToList());
                        await SqlContext.Dapper.ExecuteAsync($"UPDATE `minnings` SET `status`=0,`updatedAt`='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE `id` IN ({TaskIds})");
                    }
                    foreach (UserTaskInfo item in Users)
                    {
                        IServices.Utils.TaskSettings TaskSetting = Settings.FirstOrDefault(o => o.TaskLevel == item.TaskLevel);
                        if (item.TaskLevel >= 50 && item.TaskLevel < 60)
                        {
                            // 任务过期  释放 股权
                            SqlContext.Dapper.Execute("UPDATE user_account_equity SET Frozen = Frozen - @Shares WHERE UserId = @UserId AND Frozen >= @Shares;", new { Shares = TaskSetting.CandyIn, UserId = item.UserId });
                        }
                        if (null == TaskSetting) { continue; }
                        RspMemberRelation Relation = await Team.GetRelation(item.UserId);
                        await Team.UpdateTeamKernel(Relation.MemberId, -TaskSetting.TeamCandyH);
                    }

                    stopwatch.Stop();
                    Core.SystemLog.Jobs($"每日关闭过期任务 执行完成,执行时间:{stopwatch.Elapsed.TotalSeconds}秒");
                }
                catch (Exception ex)
                {
                    Core.SystemLog.Jobs("每日关闭过期任务 发生错误", ex);
                }
            }
        }

        private class UserTaskInfo
        {
            /// <summary>
            ///任务ID
            /// </summary>
            public int TaskId { get; set; }
            /// <summary>
            /// 用户ID
            /// </summary>
            public long UserId { get; set; }
            /// <summary>
            /// 任务等级
            /// </summary>
            public int TaskLevel { get; set; }
        }
    }
}
