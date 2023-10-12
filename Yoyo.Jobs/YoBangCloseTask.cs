using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Yoyo.Jobs
{
    /// <summary>
    /// 关闭超时任务
    /// </summary>
    public class YoBangCloseTask : IJob
    {
        private readonly IServiceProvider ServiceProvider;

        public YoBangCloseTask(IServiceProvider service)
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

                    await SqlContext.Dapper.ExecuteAsync("UPDATE yoyo_bang_record SET State = 7 WHERE NOW() > CutoffTime AND State = 1;");

                    await SqlContext.Dapper.ExecuteAsync("UPDATE yoyo_bang_task SET State = 6 WHERE Total = Complete;");
                    stopwatch.Stop();
                    Core.SystemLog.Jobs($"每日关闭YoBang过期任务 执行完成,执行时间:{stopwatch.Elapsed.TotalSeconds}秒");
                }
                catch (Exception ex)
                {
                    Core.SystemLog.Jobs("每日关闭YoBang过期任务 发生错误", ex);
                }
            }
        }
    }
}
