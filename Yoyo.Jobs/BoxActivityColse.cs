using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Yoyo.Jobs
{
    /// <summary>
    /// 宝箱算奖
    /// </summary>
    public class BoxActivityColse : IJob
    {
        private readonly IServiceProvider ServiceProvider;

        public BoxActivityColse(IServiceProvider service)
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
                    ActivityInfo activity = await SqlContext.Dapper.QueryFirstOrDefaultAsync<ActivityInfo>("SELECT * FROM yoyo_box_activity WHERE NOW() > EndTime AND State = 1;");
                    if (activity == null || activity.EndTime.AddSeconds(30) > DateTime.Now) { return; }
                    Int32 Rows = SqlContext.Dapper.Execute("UPDATE yoyo_box_activity SET State = 2 WHERE Id = @Id;", new { activity.Id });
                    if (Rows != 1) { return; }
                    BoxRecord record = await SqlContext.Dapper.QueryFirstOrDefaultAsync<BoxRecord>("SELECT * FROM yoyo_box_record WHERE Period = @Period ORDER BY Id DESC;", new { Period = activity.Period });
                    if (record == null) { return; }

                    #region 作弊配置
                    Int32 InitHours = 12;
                    Decimal InitUnitPrice = 1.10M; // 初始单价
                    Int32 InitBuyTotal = 10; // 初始购买
                    Int32 InitPeople = 1; // 初始下注人数
                    #endregion

                    SqlContext.Dapper.Open();
                    using (IDbTransaction transaction = SqlContext.Dapper.BeginTransaction())
                    {
                        try
                        {
                            #region 作弊器 最后下注人
                            Int64 WinerId = record.UserId;
                            Int64 WinRecordId = record.Id;
                            if (!String.IsNullOrWhiteSpace(activity.Remark))
                            {
                                Int64.TryParse(activity.Remark, out WinerId);
                                if (WinerId != record.UserId)
                                {
                                    WinRecordId = record.Id + 1;
                                    Decimal SingleVal = Math.Round(activity.UnitPrice / (activity.BuyTotal), 4);
                                    if (SingleVal > 0)
                                    {
                                        #region 写入分红记录
                                        StringBuilder InsertDiySql = new StringBuilder();
                                        InsertDiySql.Append("INSERT INTO `gem_records`(`userId`, `num`, `createdAt`, `updatedAt`, `description`, `gemSource`) ");
                                        InsertDiySql.Append("SELECT UserId AS userId, SUM(BuyTotal) * @SingleValue AS num, @CreateTime AS createdAt, @UpdateTime AS updatedAt, CONCAT('幸运夺宝[',SUM(BuyTotal),'把钥匙]分得',ROUND(SUM(BuyTotal) * @SingleValue, 4),'糖果') AS description, 26 AS gemSource ");
                                        InsertDiySql.Append("FROM yoyo_box_record WHERE Period = @Period AND UserId != @UserId AND Id < @RecordId GROUP BY UserId;");
                                        DynamicParameters InsertDiyParam = new DynamicParameters();
                                        InsertDiyParam.Add("UserId", WinerId, DbType.Int64);
                                        InsertDiyParam.Add("Period", activity.Period, DbType.Int32);
                                        InsertDiyParam.Add("RecordId", WinRecordId, DbType.Int64);
                                        InsertDiyParam.Add("SingleValue", SingleVal, DbType.Decimal);
                                        InsertDiyParam.Add("CreateTime", activity.EndTime.AddSeconds(-5), DbType.DateTime);
                                        InsertDiyParam.Add("UpdateTime", activity.EndTime.AddSeconds(-5), DbType.DateTime);
                                        SqlContext.Dapper.Execute(InsertDiySql.ToString(), InsertDiyParam, transaction);
                                        #endregion

                                        #region 写入账户
                                        StringBuilder UpdateDiySql = new StringBuilder();
                                        UpdateDiySql.Append("UPDATE `user` AS u, (SELECT UserId, ROUND(SUM(BuyTotal) * @SingleValue, 4) AS price FROM yoyo_box_record WHERE Period = @Period AND UserId != @UserId AND Id < @RecordId GROUP BY UserId) AS r ");
                                        UpdateDiySql.Append("SET u.candyNum = u.candyNum + r.price WHERE u.id = r.UserId;");
                                        DynamicParameters UpdateDiyParam = new DynamicParameters();
                                        UpdateDiyParam.Add("UserId", WinerId, DbType.Int64);
                                        UpdateDiyParam.Add("Period", activity.Period, DbType.Int32);
                                        UpdateDiyParam.Add("RecordId", WinRecordId, DbType.Int64);
                                        UpdateDiyParam.Add("SingleValue", SingleVal, DbType.Decimal);
                                        SqlContext.Dapper.Execute(UpdateDiySql.ToString(), UpdateDiyParam, transaction);
                                        #endregion
                                    }
                                }
                            }
                            #endregion

                            #region 大奖  分糖 50%
                            Decimal PrizeAmount = Math.Round(activity.PrizePool * 0.50M, 4);
                            StringBuilder BigPriceSql = new StringBuilder();
                            BigPriceSql.Append("INSERT INTO `gem_records`(`userId`, `num`, `createdAt`, `updatedAt`, `description`, `gemSource`) VALUES (@UserId, @UseCandy, NOW(), NOW(), @CandyDesc, @Source);");
                            BigPriceSql.Append("UPDATE `user` SET `candyNum` = `candyNum` + @UseCandy WHERE `id` = @UserId;");
                            DynamicParameters BigPriceParam = new DynamicParameters();
                            BigPriceParam.Add("UserId", WinerId, DbType.Int32);
                            BigPriceParam.Add("UseCandy", PrizeAmount, DbType.Int32);
                            BigPriceParam.Add("Source", 30, DbType.Int32);
                            BigPriceParam.Add("CandyDesc", $"夺宝最终奖：糖果{PrizeAmount.ToString("0.####")}颗", DbType.String);
                            SqlContext.Dapper.Execute(BigPriceSql.ToString(), BigPriceParam, transaction);
                            #endregion

                            #region 所有钥匙分红 35%  实际30%

                            Decimal SingleValue = Math.Round(activity.PrizePool * 0.30M / (activity.BuyTotal - record.BuyTotal), 4);
                            if (SingleValue < 0) { return; }

                            #region 写入分红记录
                            StringBuilder InsertSql = new StringBuilder();
                            InsertSql.Append("INSERT INTO `gem_records`(`userId`, `num`, `createdAt`, `updatedAt`, `description`, `gemSource`) ");
                            InsertSql.Append("SELECT UserId AS userId, SUM(BuyTotal) * @SingleValue AS num, NOW() AS createdAt, NOW() AS updatedAt, CONCAT('夺宝奖池[', SUM(BuyTotal), '把钥匙]分得', ROUND(SUM(BuyTotal) * @SingleValue, 4),'糖果') AS description, 29 AS gemSource ");
                            InsertSql.Append("FROM yoyo_box_record WHERE Period = @Period AND Id < @RecordId AND UserId != @UserId GROUP BY UserId;");
                            DynamicParameters InsertParam = new DynamicParameters();
                            InsertParam.Add("Period", activity.Period, DbType.Int64);
                            InsertParam.Add("RecordId", WinRecordId, DbType.Int64);
                            InsertParam.Add("SingleValue", SingleValue, DbType.Decimal);
                            InsertParam.Add("UserId", WinerId, DbType.Int64);
                            #endregion

                            #region 写入账户
                            StringBuilder UpdateSql = new StringBuilder();
                            UpdateSql.Append("UPDATE `user` AS u, (SELECT UserId, SUM(BuyTotal) * @SingleValue AS price FROM yoyo_box_record WHERE Period = @Period AND Id < @RecordId GROUP BY UserId) AS r ");
                            UpdateSql.Append("SET u.candyNum = u.candyNum + r.price WHERE u.id = r.UserId AND u.id != @UserId;");
                            DynamicParameters UpdateParam = new DynamicParameters();
                            UpdateParam.Add("Period", activity.Period, DbType.Int64);
                            UpdateParam.Add("RecordId", WinRecordId, DbType.Int64);
                            UpdateParam.Add("SingleValue", SingleValue, DbType.Decimal);
                            UpdateParam.Add("UserId", WinerId, DbType.Int64);
                            #endregion

                            SqlContext.Dapper.Execute(InsertSql.ToString(), InsertParam, transaction);
                            SqlContext.Dapper.Execute(UpdateSql.ToString(), UpdateParam, transaction);
                            #endregion

                            #region 写入中奖人
                            StringBuilder InsertWinerSql = new StringBuilder();
                            InsertWinerSql.Append("INSERT INTO `yoyo_box_winer`(`Period`, `RecordId`, `UserId`, `Award`, `Dividend`, `SingleValue`) ");
                            InsertWinerSql.Append("VALUES (@Period, @RecordId, @UserId, @Award, @Dividend, @SingleValue);");
                            DynamicParameters InsertWinerParam = new DynamicParameters();
                            InsertWinerParam.Add("Period", activity.Period, DbType.Int32);
                            InsertWinerParam.Add("RecordId", WinRecordId, DbType.Int64);
                            InsertWinerParam.Add("UserId", WinerId, DbType.Int64);
                            InsertWinerParam.Add("Award", PrizeAmount, DbType.Decimal);
                            InsertWinerParam.Add("Dividend", activity.PrizePool * 0.35M, DbType.Decimal);
                            InsertWinerParam.Add("SingleValue", SingleValue, DbType.Decimal);
                            SqlContext.Dapper.Execute(InsertWinerSql.ToString(), InsertWinerParam, transaction);
                            #endregion

                            #region 新加活动 5% 
                            StringBuilder NewActivity = new StringBuilder();
                            NewActivity.Append("INSERT INTO `yoyo_box_activity`(`Period`, `PrizePool`, `UnitPrice`, `BuyTotal`, `EndTime`, `State`) ");
                            NewActivity.Append("VALUES (@Period, @PrizePool, @UnitPrice, @BuyTotal, @EndTime, @State);");

                            DynamicParameters NewParam = new DynamicParameters();
                            NewParam.Add("Period", activity.Period + 1, DbType.Int32);
                            NewParam.Add("PrizePool", activity.PrizePool * 0.05M, DbType.Decimal);
                            NewParam.Add("UnitPrice", InitUnitPrice, DbType.Decimal);
                            NewParam.Add("BuyTotal", InitBuyTotal * InitPeople, DbType.Int32);
                            NewParam.Add("EndTime", DateTime.Now.AddHours(InitHours).AddMinutes(InitPeople * 5), DbType.DateTime);
                            NewParam.Add("State", 1, DbType.Int32);
                            SqlContext.Dapper.Execute(NewActivity.ToString(), NewParam, transaction);
                            #endregion

                            #region 作弊器 下注人
                            StringBuilder NewRecordSql = new StringBuilder();
                            DynamicParameters NewRecordParam = new DynamicParameters();
                            NewRecordSql.Append("INSERT INTO `yoyo_box_record`(`Period`, `UserId`, `UnitPrice`, `BuyTotal`) ");
                            NewRecordSql.Append("SELECT @Period AS Period, UserId, @UnitPrice AS UnitPrice, @BuyTotal AS BuyTotal FROM yoyo_luckydraw_defaultuser ORDER BY RAND() LIMIT @InitPeople;");
                            NewRecordParam.Add("Period", activity.Period + 1, DbType.Int32);
                            NewRecordParam.Add("UnitPrice", InitUnitPrice, DbType.Decimal);
                            NewRecordParam.Add("BuyTotal", InitBuyTotal, DbType.Int32);
                            NewRecordParam.Add("InitPeople", InitPeople, DbType.Int32);

                            SqlContext.Dapper.Execute(NewRecordSql.ToString(), NewRecordParam, transaction);
                            #endregion

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Yoyo.Core.SystemLog.Debug("宝箱算奖失败", ex);
                        }
                    }
                    SqlContext.Dapper.Close();
                }
                catch (Exception ex)
                {
                    Yoyo.Core.SystemLog.Debug("算奖失败", ex);
                }

            }
        }
    }

    public class ActivityInfo
    {
        public Int64 Id { get; set; }
        /// <summary>
        /// 期次
        /// </summary>
        public Int32 Period { get; set; }
        /// <summary>
        /// 奖池金额
        /// </summary>
        public Decimal PrizePool { get; set; }
        /// <summary>
        /// 当前单价
        /// </summary>
        public Decimal UnitPrice { get; set; }
        /// <summary>
        /// 购买总量
        /// </summary>
        public Int32 BuyTotal { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 活动状态
        /// </summary>
        public Int32 State { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public String Remark { get; set; }
    }
    /// <summary>
    /// 活动记录
    /// </summary>
    public class BoxRecord
    {
        public Int64 Id { get; set; }
        /// <summary>
        /// 期次
        /// </summary>
        public Int32 Period { get; set; }
        /// <summary>
        /// 会员编号
        /// </summary>
        public Int64 UserId { get; set; }
        /// <summary>
        /// 购买单价
        /// </summary>
        public Decimal UnitPrice { get; set; }
        /// <summary>
        /// 购买总量
        /// </summary>
        public Int32 BuyTotal { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public String Remark { get; set; }
    }
}
