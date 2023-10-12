using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Yoyo.Core.Expand;

namespace Yoyo.Jobs
{
    public class DailyNewInviteRanking : IJob
    {
        private readonly IServiceProvider ServiceProvider;

        public DailyNewInviteRanking(IServiceProvider service)
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


                    #region 直推排行榜分红
                    //=======================取今日交易分红信息=======================//
                    Entity.Models.EverydayDividend Dividend = await SqlContext.EverydayDividend.FirstOrDefaultAsync(o => o.DividendDate == DateTime.Now.Date);
                    if (null != Dividend)
                    {
                        #region 新邀请排行榜
                        Int32 Phase = DateTime.Now.Month;   //当前期
                        if (DateTime.Now.Date.Day == 1)
                        {
                            Phase = DateTime.Now.AddMonths(-1).Month;
                        }

                        //=======本日所有用户
                        int TotalUser = SqlContext.MemberInviteRanking.Where(o => o.InviteDate.Date == DateTime.Now.Date).Sum(oo => oo.InviteToday);
                        List<Entity.Models.MemberInviteRanking> InviteUsers = await SqlContext.MemberInviteRanking.Where(o => o.Phase == Phase && o.InviteToday >= 5 && o.InviteDate.Date == DateTime.Now.Date).ToListAsync();
                        int OkUser = InviteUsers.Sum(oo => oo.InviteToday);

                        //==本日分红信息

                        var OnePeople = Dividend.CandyFee * 0.05M / TotalUser;


                        StringBuilder GiveCandy = new StringBuilder();
                        StringBuilder RecordCandy = new StringBuilder("insert into `gem_records`(`userId`,`num`,`description`,gemSource) values ");
                        bool UseRecord = false;
                        InviteUsers.ForEach(x =>
                        {
                            var currentGive = OnePeople * x.InviteToday;
                            //循环  算出每个人分多少
                            GiveCandy.AppendLine($"update `user` set candyNum = (candyNum + {currentGive}) where id = {x.UserId};");
                            RecordCandy.Append($"\r\n({x.UserId},{currentGive},'拉新分红,邀请用户:{x.InviteToday}',22),"); //gemSource 22代表最新版的排行榜奖励
                            UseRecord = true;
                        });
                        String RecordCandySqlString = RecordCandy.ToString().TrimEnd(',');
                        String GiveCandySqlString = GiveCandy.ToString();
                        if (UseRecord) 
                        {
                            using (IDbConnection db = SqlContext.DapperConnection)
                            {
                                db.Open();
                                IDbTransaction Tran = db.BeginTransaction();
                                try
                                {
                                    int UpdateRows = db.Execute(GiveCandySqlString, null, Tran);
                                    int InsertRows = db.Execute(RecordCandySqlString, null, Tran);
                                    Tran.Commit();
                                }
                                catch (Exception ex)
                                {
                                    System.Console.WriteLine(ex);
                                    Tran.Rollback();
                                }
                                finally { if (db.State == ConnectionState.Open) { db.Close(); } }
                            }
                        }
                        
                        #endregion

                    }
                    #endregion

                    stopwatch.Stop();
                    Core.SystemLog.Jobs($"每日邀请排行榜分红 执行完成,执行时间:{stopwatch.Elapsed.TotalSeconds}秒");
                }
                catch (Exception ex)
                {
                    Core.SystemLog.Jobs("每日邀请排行榜分红 发生错误", ex);
                }
            }
        }
    }
}
