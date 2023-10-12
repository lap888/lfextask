using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl.Triggers;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yoyo.Core
{
    /// <summary>
    /// 定时任务初始化
    /// </summary>
    public class JobScheduler
    {
        private IScheduler scheduler;
        private readonly IJobFactory TaskFactory;
        private readonly ISchedulerFactory SchedulerFactory;
        private readonly List<JobDetail> TaskCrons;
        private const String TaskGroupLastName = "_Group";
        private const String TriggerNameLast = "_Trigger";
        private const String TriggerGroupLastName = "_TriggerGroup";

        public JobScheduler(IJobFactory job, ISchedulerFactory scheduler, IOptionsMonitor<List<JobDetail>> options)
        {
            this.SchedulerFactory = scheduler;
            this.TaskFactory = job;
            this.TaskCrons = options.CurrentValue;
        }
        /// <summary>
        /// 启动任务调度中心
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            this.scheduler = await this.SchedulerFactory.GetScheduler();
            this.scheduler.JobFactory = this.TaskFactory;
            await this.scheduler.Start();
        }
        /// <summary>
        /// 停止任务调度
        /// </summary>
        public void Stop()
        {
            if (null == this.scheduler) { return; }
            if (this.scheduler.Shutdown(true).Wait(30 * 1000)) { this.scheduler = null; }
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <typeparam name="T">任务内容</typeparam>
        /// <returns>添加结果</returns>
        public async Task<Boolean> AddTask<T>() where T : class, IJob
        {
            try
            {
                String jobName = typeof(T).Name;
                JobDetail jobDetail = this.TaskCrons.FirstOrDefault(o => o.JobName == jobName);
                if (null == jobDetail) { throw new ArgumentNullException($"任务配置项内，找不到 {jobName} 这个任务,请添加配置后重新启动项目。"); }
                if (!jobDetail.IsEnable) { return false; }
                if (!CronExpression.IsValidExpression(jobDetail.CronTime)) { throw new FormatException($"任务 {jobName} 的Cron调度表达式非法。"); }
                IJobDetail job = JobBuilder.Create<T>().WithIdentity(jobDetail.JobName, jobDetail.JobName + TaskGroupLastName).Build();
                ICronTrigger trigger = new CronTriggerImpl(jobDetail.JobName + TriggerNameLast, jobDetail.JobName + TriggerGroupLastName, jobDetail.CronTime);
                DateTimeOffset timer = await this.scheduler.ScheduleJob(job, trigger);
            }
            catch (Exception ex)
            {
                Core.SystemLog.Jobs("定时任务添加失败", ex);
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// 任务详情
    /// </summary>
    public class JobDetail
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public String JobName { get; set; }
        /// <summary>
        /// Cron表达式
        /// </summary>
        public String CronTime { get; set; }
        /// <summary>
        /// 是否启动
        /// </summary>
        public Boolean IsEnable { get; set; }
    }

    /// <summary>
    /// 任务工厂服务
    /// </summary>
    public class JobFactoryService : IJobFactory
    {
        private readonly IServiceProvider ServiceProvider;

        public JobFactoryService(IServiceProvider service)
        {
            this.ServiceProvider = service;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return this.ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            IDisposable disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}
