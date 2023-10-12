using Quartz;
using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Dapper;
using Yoyo.IServices.ICityPartner;

namespace Yoyo.Jobs
{
    public class ShandwDevidend : IJob
    {
        // private readonly ShandwConfig config;
        private readonly IServiceProvider ServiceProvider;

        public ShandwDevidend(IServiceProvider service)
        {
            this.ServiceProvider = service;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var service = this.ServiceProvider.CreateScope())
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                Entity.SqlContext SqlContext = service.ServiceProvider.GetRequiredService<Entity.SqlContext>();
                ICityDividend DividendSub = service.ServiceProvider.GetRequiredService<ICityDividend>();

                try
                {
                    IEnumerable<Entity.Models.ShandwOrder> orders = await SqlContext.Dapper.QueryAsync<Entity.Models.ShandwOrder>("SELECT * FROM yoyo_shandw_order WHERE State = 1 ORDER BY Id DESC;");

                    foreach (var item in orders)
                    {
                        await DividendSub.ShandwDividend(new IServices.Models.DividendModel()
                        {
                            DividendType = Entity.Enums.DividendType.Shandw,
                            RecordId = item.Id
                        });
                    }
                }
                catch (Exception ex)
                {
                    Yoyo.Core.SystemLog.Debug("游戏订单分红===>>失败", ex);
                }
            }
        }



    }
}
