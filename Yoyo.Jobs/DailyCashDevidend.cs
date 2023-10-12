using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoyo.Jobs
{
    public class DailyCashDevidend : IJob
    {
        private readonly IServiceProvider ServiceProvider;

        public DailyCashDevidend(IServiceProvider service)
        {
            this.ServiceProvider = service;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            StringBuilder Sql = new StringBuilder();
            Sql.AppendLine("SELECT u.id,u.candyNum,w.AccountId AS monthlyTradeCount FROM (");
            Sql.AppendLine("SELECT userId,COUNT(1) AS Tasks FROM minnings WHERE `status`=1 AND minningId IN (9,8,4,105) AND TO_DAYS(Now())>=TO_DAYS(`beginTime`) AND TO_DAYS(Now())<TO_DAYS(`endTime`)  GROUP BY userId) AS t ");
            Sql.AppendLine("INNER JOIN (SELECT * FROM `user` WHERE auditState=2 AND `status`=0 AND candyNum>=1000) AS u ON t.userId=u.id ");
            Sql.AppendLine("INNER JOIN user_account_wallet AS w ON t.userId=w.UserId");


            using (var service = this.ServiceProvider.CreateScope())
            {
                try
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    Entity.SqlContext SqlContext = service.ServiceProvider.GetRequiredService<Entity.SqlContext>();

                    var AllCash = 0M;

                    AllCash = SqlContext.Dapper.QueryFirstOrDefault<decimal>($"SELECT Cash FROM yoyo_cash_dividend WHERE CashDate='{DateTime.Now.ToString("yyyy-MM-dd")}'");

                    if (AllCash <= 0) { return; }

                    var Users = (await SqlContext.Dapper.QueryAsync<Entity.Models.User>(Sql.ToString())).ToList();
                    if (Users.Count == 0) { return; }

                    var AllCandy = Users.Sum(o => o.CandyNum);

                    var OneCandyToCash = ((int)(AllCash / AllCandy * 10000)) * 0.0001M;

                    //写入单个糖果价值
                    SqlContext.Dapper.Execute("UPDATE yoyo_cash_dividend SET SingleCandy = @OneCandyToCash WHERE CashDate = @CashDate;", new { OneCandyToCash, CashDate = DateTime.Now.ToString("yyyy-MM-dd") });

                    if (OneCandyToCash <= 0) { return; }

                    foreach (var item in Users)
                    {
                        var UserGiveCash = ((int)item.CandyNum) * OneCandyToCash;
                        ChangeWalletAmount(SqlContext.DapperConnection, item.MonthlyTradeCount.Value, UserGiveCash, ((int)item.CandyNum).ToString(), OneCandyToCash.ToString("F4"));
                    }


                }
                catch (Exception ex)
                {
                    Core.SystemLog.Jobs("每日现金分红 发生错误", ex);
                }
            }
        }

        private void ChangeWalletAmount(IDbConnection DataBase, long AccountId, decimal Amount, params string[] Desc)
        {
            String EditSQl, RecordSql, PostChangeSql;

            EditSQl = $"UPDATE `user_account_wallet` SET `Balance`=`Balance`+{Amount},`Revenue`=`Revenue`+{Math.Abs(Amount)},`ModifyTime`=NOW() WHERE `AccountId`={AccountId}";

            PostChangeSql = $"IFNULL((SELECT `PostChange` FROM `user_account_wallet_record` WHERE `AccountId`={AccountId} ORDER BY `RecordId` DESC LIMIT 1),0)";
            StringBuilder TempRecordSql = new StringBuilder($"INSERT INTO `user_account_wallet_record` ");
            TempRecordSql.Append("( `AccountId`, `PreChange`, `Incurred`, `PostChange`, `ModifyType`, `ModifyDesc`, `ModifyTime` ) ");
            TempRecordSql.Append($"SELECT {AccountId} AS `AccountId`, ");
            TempRecordSql.Append($"{PostChangeSql} AS `PreChange`, ");
            TempRecordSql.Append($"{Amount} AS `Incurred`, ");
            TempRecordSql.Append($"{PostChangeSql}+{Amount} AS `PostChange`, ");
            TempRecordSql.Append($"{5} AS `ModifyType`, ");
            TempRecordSql.Append($"'{String.Join(',', Desc)}' AS `ModifyDesc`, ");
            TempRecordSql.Append($"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' AS`ModifyTime`");
            RecordSql = TempRecordSql.ToString();

            #region 修改账务
            using (IDbConnection db = DataBase)
            {
                db.Open();
                IDbTransaction Tran = db.BeginTransaction();
                try
                {
                    Int32 EditRow = db.Execute(EditSQl, null, Tran);
                    Int32 RecordId = db.Execute(RecordSql, null, Tran);
                    if (EditRow == RecordId && EditRow == 1) { Tran.Commit(); return; }
                    Tran.Rollback();
                    Core.SystemLog.Debug($"钱包账户余额变更发生错误\r\n修改语句：\r\n{EditSQl}\r\n记录语句：{RecordSql}");
                }
                catch (Exception ex)
                {
                    Tran.Rollback();
                    Core.SystemLog.Debug($"钱包账户余额变更发生错误\r\n修改语句：\r\n{EditSQl}\r\n记录语句：{RecordSql}", ex);
                }
                finally { if (db.State == ConnectionState.Open) { db.Close(); } }
            }
            #endregion
        }
    }
}
