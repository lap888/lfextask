using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Yoyo.Entity.Enums;
using System.Linq;

namespace Yoyo.Jobs
{
    /// <summary>
    /// 城市合伙人 分红
    /// </summary>
    public class CityPartnerDividend : IJob
    {
        private readonly IServiceProvider ServiceProvider;
        public CityPartnerDividend(IServiceProvider service)
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

                try
                {
                    Int32 Week = await GetWeek();
                    DividendType type = DividendType.YoBang;
                    Int32 ModifyType = 99;
                    DynamicParameters DividendSqlParam = new DynamicParameters();
                    switch (Week)
                    {
                        case 1:
                            type = DividendType.YoBang;
                            DividendSqlParam.Add("DividendType", (Int32)type, DbType.Int32);
                            DividendSqlParam.Add("DividendState", (Int32)DividendState.Normal, DbType.Int32);
                            DividendSqlParam.Add("DividendDate", DateTime.Now.AddDays(-Week).Date, DbType.Date);
                            DividendSqlParam.Add("DividendDesc", "-哟帮分红：", DbType.String);
                            DividendSqlParam.Add("GemSource", 55, DbType.Int32);
                            DividendSqlParam.Add("ModifyType", 15, DbType.Int32);
                            ModifyType = 15;
                            break;
                        case 2:
                            type = DividendType.Video;
                            DividendSqlParam.Add("DividendType", (Int32)type, DbType.Int32);
                            DividendSqlParam.Add("DividendState", (Int32)DividendState.Normal, DbType.Int32);
                            DividendSqlParam.Add("DividendDate", DateTime.Now.AddDays(-Week).Date, DbType.Date);
                            DividendSqlParam.Add("DividendDesc", "-视频分红：", DbType.String);
                            DividendSqlParam.Add("GemSource", 56, DbType.Int32);
                            DividendSqlParam.Add("ModifyType", 16, DbType.Int32);
                            ModifyType = 16;
                            break;
                        case 3:
                            type = DividendType.Shandw;
                            DividendSqlParam.Add("DividendType", (Int32)type, DbType.Int32);
                            DividendSqlParam.Add("DividendState", (Int32)DividendState.Normal, DbType.Int32);
                            DividendSqlParam.Add("DividendDate", DateTime.Now.AddDays(-Week).Date, DbType.Date);
                            DividendSqlParam.Add("DividendDesc", "-游戏分红：", DbType.String);
                            DividendSqlParam.Add("GemSource", 57, DbType.Int32);
                            DividendSqlParam.Add("ModifyType", 17, DbType.Int32);
                            ModifyType = 17;
                            break;
                        case 4:
                            type = DividendType.Mall;
                            DividendSqlParam.Add("DividendType", (Int32)type, DbType.Int32);
                            DividendSqlParam.Add("DividendState", (Int32)DividendState.Normal, DbType.Int32);
                            DividendSqlParam.Add("DividendDate", DateTime.Now.AddDays(-Week).Date, DbType.Date);
                            DividendSqlParam.Add("DividendDesc", "-商城分红：", DbType.String);
                            DividendSqlParam.Add("GemSource", 58, DbType.Int32);
                            DividendSqlParam.Add("ModifyType", 18, DbType.Int32);
                            ModifyType = 18;
                            break;
                        default:
                            return;
                    }

                    #region 糖果分红

                    #region 查询城主信息SQL
                    StringBuilder QueryCityCandySql = new StringBuilder();
                    QueryCityCandySql.Append("SELECT acc.Id, acc.CityNo, city.UserId, acc.Amount FROM city_candy_dividend AS acc, yoyo_city_master AS city ");
                    QueryCityCandySql.Append("WHERE acc.CityNo = city.CityCode ");
                    QueryCityCandySql.Append("AND DividendType = @DividendType AND @DividendDate BETWEEN acc.StartDate AND acc.EndDate AND acc.State = @DividendState ");
                    #endregion

                    #region 写入糖果账户
                    StringBuilder CandyDividendSql = new StringBuilder();
                    CandyDividendSql.Append("UPDATE `user` AS u, (");
                    CandyDividendSql.Append(QueryCityCandySql);
                    CandyDividendSql.Append(") AS c ");
                    CandyDividendSql.Append("SET u.candyNum = u.candyNum + c.Amount WHERE u.id = c.UserId;");
                    //写入记录
                    CandyDividendSql.Append("INSERT INTO `gem_records`(`userId`, `num`, `createdAt`, `updatedAt`, `description`, `gemSource`) ");
                    CandyDividendSql.Append("SELECT city.UserId AS userId, acc.Amount AS num, NOW() AS createdAt, NOW() AS updatedAt, ");
                    CandyDividendSql.Append("CONCAT('[', acc.CityNo, ']城主',@DividendDesc, ROUND(acc.Amount, 4),'糖果') AS description, @GemSource AS gemSource ");
                    CandyDividendSql.Append("FROM city_candy_dividend AS acc, yoyo_city_master AS city WHERE acc.CityNo = city.CityCode AND DividendType = @DividendType ");
                    CandyDividendSql.Append("AND @DividendDate BETWEEN acc.StartDate AND acc.EndDate; ");
                    #endregion

                    #region 更新城主 糖果账户
                    CandyDividendSql.Append("UPDATE city_earnings AS e, (");
                    CandyDividendSql.Append(QueryCityCandySql);
                    CandyDividendSql.Append(") AS c ");
                    CandyDividendSql.Append("SET e.Candy = e.Candy + c.Amount WHERE e.CityNo = c.CityNo;");
                    #endregion

                    #endregion

                    #region 现金分红

                    #region 查询城市信息SQL
                    StringBuilder QueryCitySql = new StringBuilder();
                    QueryCitySql.Append("SELECT acc.Id, acc.CityNo, city.UserId, acc.Amount FROM city_cash_dividend AS acc, yoyo_city_master AS city ");
                    QueryCitySql.Append("WHERE acc.CityNo = city.CityCode ");
                    QueryCitySql.Append("AND DividendType = @DividendType AND @DividendDate BETWEEN acc.StartDate AND acc.EndDate AND acc.State = @DividendState ");
                    #endregion

                    #region 为不存在红包账户的城主  创建账户
                    StringBuilder CreateWalletSql = new StringBuilder();
                    CreateWalletSql.Append("INSERT INTO `user_account_wallet` (`UserId`, `Revenue`, `Expenses`, `Balance`, `Frozen`, `ModifyTime`) ");
                    CreateWalletSql.Append("SELECT city.UserId, 0 AS Revenue,0 AS Expenses,0 AS Balance,0 AS Frozen, NOW() AS ModifyTime FROM user_account_wallet AS red, (");
                    CreateWalletSql.Append(QueryCitySql);
                    CreateWalletSql.Append(") AS city ");
                    CreateWalletSql.Append("WHERE red.UserId = city.UserId AND ISNULL(red.UserId);");
                    #endregion

                    #region 写入现金账户
                    //StringBuilder CashDividendSql = new StringBuilder();
                    //CashDividendSql.Append("UPDATE user_account_wallet AS u, (");
                    //CashDividendSql.Append(QueryCitySql);
                    //CashDividendSql.Append(") AS c ");
                    //CashDividendSql.Append("SET u.Balance = u.Balance + c.Amount WHERE u.UserId = c.UserId;");
                    ////写入现金记录
                    //CashDividendSql.Append("INSERT INTO `user_account_wallet_record`(`AccountId`, `PreChange`, `Incurred`, `PostChange`, `ModifyType`, `ModifyDesc`, `ModifyTime`) ");
                    //CashDividendSql.Append("SELECT a.AccountId, a.PreChange, b.Amount AS Incurred, a.PreChange + b.Amount AS PostChange, @ModifyType AS ModifyType, b.CityNo AS ModifyDesc, NOW() AS ModifyTime FROM (");
                    //CashDividendSql.Append("SELECT MAX( r.RecordId ), w.AccountId, w.UserId, IFNULL( r.PostChange, 0 ) AS PreChange FROM user_account_wallet AS w ");
                    //CashDividendSql.Append("LEFT JOIN user_account_wallet_record AS r ON r.AccountId = w.AccountId GROUP BY r.AccountId) AS a, (");
                    //CashDividendSql.Append(QueryCitySql);
                    //CashDividendSql.Append(") AS b WHERE a.UserId = b.UserId; ");
                    #endregion

                    #region 更新城主 现金账户

                    IEnumerable<CityAccount> CashDividends = SqlContext.Dapper.Query<CityAccount>(QueryCitySql.ToString(), DividendSqlParam);
                    foreach (var item in CashDividends)
                    {
                        ChangeWalletAmount(SqlContext.DapperConnection, item.UserId, ModifyType, item.Amount, item.CityNo);
                    }

                    StringBuilder CashDividendSql = new StringBuilder();
                    CashDividendSql.Append("UPDATE city_earnings AS e, (");
                    CashDividendSql.Append(QueryCitySql);
                    CashDividendSql.Append(") AS c ");
                    CashDividendSql.Append("SET e.Cash = e.Cash + c.Amount WHERE e.CityNo = c.CityNo;");
                    #endregion

                    #endregion

                    #region 修改分红状态

                    StringBuilder ColseDividend = new StringBuilder();
                    ColseDividend.Append("UPDATE city_candy_dividend AS acc, yoyo_city_master AS city SET acc.State = 2 WHERE acc.CityNo = city.CityCode ");
                    ColseDividend.Append("AND acc.DividendType = @DividendType AND @DividendDate BETWEEN acc.StartDate AND acc.EndDate AND acc.State = @DividendState;");
                    ColseDividend.Append("UPDATE city_cash_dividend AS acc, yoyo_city_master AS city SET acc.State = 2 WHERE acc.CityNo = city.CityCode ");
                    ColseDividend.Append("AND acc.DividendType = @DividendType AND @DividendDate BETWEEN acc.StartDate AND acc.EndDate AND acc.State = @DividendState;");

                    #endregion

                    SqlContext.Dapper.Open();
                    using (IDbTransaction transaction = SqlContext.Dapper.BeginTransaction())
                    {
                        try
                        {
                            SqlContext.Dapper.Execute(CandyDividendSql.ToString(), DividendSqlParam, transaction);
                            SqlContext.Dapper.Execute(CreateWalletSql.ToString(), DividendSqlParam, transaction);
                            SqlContext.Dapper.Execute(CashDividendSql.ToString(), DividendSqlParam, transaction);
                            SqlContext.Dapper.Execute(ColseDividend.ToString(), DividendSqlParam, transaction);

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Core.SystemLog.Error($"城市合伙人 ==> 分红\r\n周{Week.ToString()}分红", ex);
                            throw ex;
                        }
                        finally
                        {
                            if (SqlContext.Dapper.State == ConnectionState.Open) { SqlContext.Dapper.Close(); }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Yoyo.Core.SystemLog.Debug("城市合伙人分红===>>失败", ex);
                }
            }
        }

        /// <summary>
        /// 城市账户
        /// </summary>
        public class CityAccount
        {
            /// <summary>
            /// 编号
            /// </summary>
            public Int64 Id { get; set; }

            /// <summary>
            /// 城市编号
            /// </summary>
            public String CityNo { get; set; }

            /// <summary>
            /// 会员编号
            /// </summary>
            public Int64 UserId { get; set; }

            /// <summary>
            /// 金额
            /// </summary>
            public Decimal Amount { get; set; }
        }

        /// <summary>
        /// 获取星期几
        /// </summary>
        /// <param name="Dt"></param>
        /// <returns></returns>
        private async Task<Int32> GetWeek(DateTime? Dt = null)
        {
            return await Task.Run(() =>
            {
                if (Dt == null) { Dt = DateTime.Now; }
                Int32 Week = (Int32)Dt?.DayOfWeek;
                if (Week == 0) { Week = 7; }
                return Week;
            });
        }


        /// <summary>
        /// 钱包账务
        /// </summary>
        /// <param name="DataBase">数据库</param>
        /// <param name="UserId">用户ID</param>
        /// <param name="ModifyType">变更类型</param>
        /// <param name="Amount">金额</param>
        /// <param name="Desc">描述</param>
        private void ChangeWalletAmount(IDbConnection DataBase, long UserId, int ModifyType, decimal Amount, params string[] Desc)
        {
            String EditSQl, RecordSql, PostChangeSql;

            long AccountId;

            AccountId = DataBase.QueryFirst<long>($"SELECT AccountId FROM user_account_wallet WHERE UserId={UserId}");

            EditSQl = $"UPDATE `user_account_wallet` SET `Balance`=`Balance`+{Amount},`Revenue`=`Revenue`+{Math.Abs(Amount)},`ModifyTime`=NOW() WHERE `AccountId`={AccountId}";

            PostChangeSql = $"IFNULL((SELECT `PostChange` FROM `user_account_wallet_record` WHERE `AccountId`={AccountId} ORDER BY `RecordId` DESC LIMIT 1),0)";
            StringBuilder TempRecordSql = new StringBuilder($"INSERT INTO `user_account_wallet_record` ");
            TempRecordSql.Append("( `AccountId`, `PreChange`, `Incurred`, `PostChange`, `ModifyType`, `ModifyDesc`, `ModifyTime` ) ");
            TempRecordSql.Append($"SELECT {AccountId} AS `AccountId`, ");
            TempRecordSql.Append($"{PostChangeSql} AS `PreChange`, ");
            TempRecordSql.Append($"{Amount} AS `Incurred`, ");
            TempRecordSql.Append($"{PostChangeSql}+{Amount} AS `PostChange`, ");
            TempRecordSql.Append($"{ModifyType} AS `ModifyType`, ");
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
