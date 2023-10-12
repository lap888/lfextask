using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Yoyo.IServices.ICityPartner;
using Dapper;
using System.Data;
using Yoyo.IServices.Request;
using Yoyo.Entity.Models;
using Yoyo.Entity.Enums;
using Yoyo.IServices.Models;
using Newtonsoft.Json;

namespace Yoyo.Service.CityPartner
{
    public class CityDividend : ICityDividend
    {
        private readonly Entity.SqlContext SqlContext;
        private readonly CSRedis.CSRedisClient RedisCache;
        public CityDividend(Entity.SqlContext context, CSRedis.CSRedisClient redis)
        {
            SqlContext = context;
            RedisCache = redis;
        }

        /// <summary>
        /// 哟帮城市分红
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task YoBangDividend(DividendModel model)
        {
            Decimal TaskRate = 0.15M;
            try
            {
                #region 系统缓存控制
                String CacheKey = $"CityDividend_YoBang_Lock:{model.RecordId}";
                if (RedisCache.Exists(CacheKey)) { return; }
                RedisCache.Set(CacheKey, model, 300);
                #endregion

                StringBuilder QueryTaskSql = new StringBuilder();
                QueryTaskSql.Append("SELECT r.TaskId, t.Publisher, r.UserId, t.RewardType, t.UnitPrice, t.FeeRate FROM ");
                QueryTaskSql.Append("yoyo_bang_record AS r, yoyo_bang_task AS t WHERE r.TaskId = t.Id AND r.Id = @RecordId;");
                BangTaskInfo TaskInfo = await SqlContext.Dapper.QueryFirstOrDefaultAsync<BangTaskInfo>(QueryTaskSql.ToString(), new { model.RecordId });

                if (TaskInfo == null) { return; }

                #region 发布人城市 分红  手续费的10%;
                String PublisherCity = SqlContext.Dapper.QueryFirstOrDefault<String>("SELECT cityCode FROM user_locations WHERE userId = @Publisher;", new { TaskInfo.Publisher });
                if (!String.IsNullOrWhiteSpace(PublisherCity))
                {
                    Decimal PublisherCityAmount = TaskInfo.UnitPrice * TaskRate * 0.10M;
                    if (TaskInfo.RewardType == 1)
                    {
                        await CashDividend(new ReqCityDividend()
                        {
                            CityNo = PublisherCity,
                            Amount = PublisherCityAmount,
                            Type = Entity.Enums.DividendType.YoBang,
                            Desc = new String[] { model.RecordId.ToString(), TaskInfo.TaskId.ToString() }
                        });
                    }
                    else
                    {
                        await CandyDividend(new ReqCityDividend()
                        {
                            CityNo = PublisherCity,
                            Amount = PublisherCityAmount,
                            Type = Entity.Enums.DividendType.YoBang,
                            Desc = new String[] { model.RecordId.ToString(), TaskInfo.TaskId.ToString() }
                        });
                    }
                }
                #endregion

                #region 任务人城市 分红  手续费的5%;
                String TaskManCity = SqlContext.Dapper.QueryFirstOrDefault<String>("SELECT cityCode FROM user_locations WHERE userId = @UserId;", new { TaskInfo.UserId });
                if (!String.IsNullOrWhiteSpace(TaskManCity))
                {
                    Decimal TaskManCityAmount = TaskInfo.UnitPrice * TaskRate * 0.05M;
                    if (TaskInfo.RewardType == 1)
                    {
                        await CashDividend(new ReqCityDividend()
                        {
                            CityNo = TaskManCity,
                            Amount = TaskManCityAmount,
                            Type = Entity.Enums.DividendType.YoBang,
                            Desc = new String[] { model.RecordId.ToString(), TaskInfo.TaskId.ToString() }
                        });
                    }
                    else
                    {
                        await CandyDividend(new ReqCityDividend()
                        {
                            CityNo = TaskManCity,
                            Amount = TaskManCityAmount,
                            Type = Entity.Enums.DividendType.YoBang,
                            Desc = new String[] { model.RecordId.ToString(), TaskInfo.TaskId.ToString() }
                        });
                    }
                }
                #endregion
            }
            catch (Exception ex) { Core.SystemLog.Error($"城市合伙人 ==> Yo帮分红\r\n{model.ToString()}", ex); }
        }

        /// <summary>
        /// 广告城市分红
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task VideoDividend(DividendModel model)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 商城城市分红
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task MallDividend(DividendModel model)
        {
            try
            {
                Decimal DividendRate = 0.15M;

                #region 系统缓存控制
                String CacheKey = $"CityDividend_Shandw_Lock:{model.RecordId}";
                if (RedisCache.Exists(CacheKey)) { return; }
                RedisCache.Set(CacheKey, model, 300);
                #endregion

                #region 写入订单
                Int64 UserId = 0;
                TrefoilOrder order = JsonConvert.DeserializeObject<TrefoilOrder>(model.Remark);
                if (order == null) { return; }
                Int64.TryParse(order.UnionCustom, out UserId);

                StringBuilder InsertOrder = new StringBuilder();
                DynamicParameters InsertParam = new DynamicParameters();
                InsertOrder.Append("INSERT INTO `yoyo_mall_order`(`UserId`, `UnionNo`, `UnionType`, `UnionPid`, `UnionCustom`, `GoodsId`, `GoodsName`, `GoodsImage`, `GoodsPrice`, `GoodsQuantity`, `OrderAmount`, `Commission`, `OrderStatus`, `CreateTime`, `ModifyTime`) ");
                InsertOrder.Append("VALUES (@UserId, @UnionNo, @UnionType, @UnionPid, @UnionCustom, @GoodsId, @GoodsName, @GoodsImage, @GoodsPrice, @GoodsQuantity, @OrderAmount, @Commission, @OrderStatus, NOW(), NOW());select @@IDENTITY");
                InsertParam.Add("UserId", UserId, DbType.Int64);
                InsertParam.Add("UnionNo", order.UnionNo, DbType.String);
                InsertParam.Add("UnionType", order.UnionType, DbType.Int32);
                InsertParam.Add("UnionPid", order.UnionPid, DbType.String);
                InsertParam.Add("UnionCustom", order.UnionCustom, DbType.String);
                InsertParam.Add("GoodsId", order.GoodsId, DbType.Int64);
                InsertParam.Add("GoodsName", order.GoodsName, DbType.String);
                InsertParam.Add("GoodsImage", order.GoodsImage, DbType.String);
                InsertParam.Add("GoodsPrice", order.GoodsPrice * 0.01M, DbType.Decimal);
                InsertParam.Add("GoodsQuantity", order.GoodsQuantity, DbType.Int32);
                InsertParam.Add("OrderAmount", order.OrderAmount * 0.01M, DbType.Decimal);
                InsertParam.Add("Commission", order.UserCommission * 0.01M, DbType.Decimal);
                InsertParam.Add("OrderStatus", order.OrderStatus, DbType.Int32);
                Int64 RecordId = await SqlContext.Dapper.ExecuteScalarAsync<Int64>(InsertOrder.ToString(), InsertParam);
                #endregion

                if (UserId < 1) { return; }

                String UserCity = await SqlContext.Dapper.QueryFirstOrDefaultAsync<String>("SELECT cityCode FROM user_locations WHERE userId = @UserId;", new { UserId });
                if (!String.IsNullOrWhiteSpace(UserCity))
                {
                    await CashDividend(new ReqCityDividend()
                    {
                        CityNo = UserCity,
                        Amount = Math.Round(order.UserCommission * 0.01M * DividendRate, 4),
                        Desc = new String[] { RecordId.ToString() },
                        Type = DividendType.Mall,
                    });
                }
            }
            catch (Exception ex)
            {
                Core.SystemLog.Error($"城市合伙人 ==> 商城分红\r\n{model.ToString()}", ex);
            }
        }

        /// <summary>
        /// 游戏分红
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task ShandwDividend(DividendModel model)
        {
            try
            {
                Decimal DividendRate = 0.15M;

                #region 系统缓存控制
                String CacheKey = $"CityDividend_Shandw_Lock:{model.RecordId}";
                if (RedisCache.Exists(CacheKey)) { return; }
                RedisCache.Set(CacheKey, model, 300);
                #endregion

                ShandwOrder order = SqlContext.Dapper.QueryFirstOrDefault<ShandwOrder>("SELECT Id, UserId, PayMoney, Amount, State FROM yoyo_shandw_order WHERE Id = @RecordId AND State = @State;", new { State = (Int32)DividendState.Normal, model.RecordId });
                String UserCity = await SqlContext.Dapper.QueryFirstOrDefaultAsync<String>("SELECT cityCode FROM user_locations WHERE userId = @UserId;", new { order.UserId });
                if (order == null) { return; }
                if (!String.IsNullOrWhiteSpace(UserCity))
                {
                    await CashDividend(new ReqCityDividend()
                    {
                        CityNo = UserCity,
                        Amount = Math.Round(order.Amount * DividendRate, 4),
                        Desc = new String[] { order.Id.ToString() },
                        Type = DividendType.Shandw,
                    });
                    SqlContext.Dapper.Execute("UPDATE yoyo_shandw_order SET State = @State WHERE Id = @RecordId", new { RecordId = order.Id, State = (Int32)DividendState.Dividends });
                }
                else
                {
                    SqlContext.Dapper.Execute("UPDATE yoyo_shandw_order SET State = @State WHERE Id = @RecordId", new { RecordId = order.Id, State = (Int32)DividendState.NotCity });
                }
            }
            catch (Exception ex)
            {
                Yoyo.Core.SystemLog.Debug($"游戏分红==>>{JsonConvert.SerializeObject(model)}", ex);
            }
        }

        /// <summary>
        /// 现金分红
        /// </summary>
        /// <param name="dividend"></param>
        /// <returns></returns>
        public async Task CashDividend(ReqCityDividend dividend)
        {
            #region 获取本期 账户
            CityCashDividend DividendAccount;
            StringBuilder QueryCitySql = new StringBuilder();
            QueryCitySql.Append("SELECT * FROM city_cash_dividend WHERE CityNo = @CityNo AND DividendType = @DividendType AND NOW() BETWEEN StartDate AND EndDate;");
            DividendAccount = await SqlContext.Dapper.QueryFirstOrDefaultAsync<CityCashDividend>(QueryCitySql.ToString(), new { dividend.CityNo, DividendType = (Int32)dividend.Type });
            if (DividendAccount == null)
            {
                DividendAccount = new CityCashDividend()
                {
                    CityNo = dividend.CityNo,
                    DividendType = dividend.Type,
                    Amount = 0,
                    StartDate = GetWeekDate((Int32)dividend.Type).Date,
                    EndDate = GetWeekDate((Int32)dividend.Type + 7).Date,
                    State = DividendState.Normal,
                    Remark = String.Empty
                };
                StringBuilder InsertCity = new StringBuilder();
                DynamicParameters InsertParam = new DynamicParameters();
                InsertCity.Append("INSERT INTO `city_cash_dividend`(`CityNo`, `DividendType`, `Amount`, `StartDate`, `EndDate`, `State`, `Remark`) ");
                InsertCity.Append("VALUES (@CityNo, @DividendType, @Amount, @StartDate, @EndDate, @State, @Remark);select @@IDENTITY");
                InsertParam.Add("CityNo", DividendAccount.CityNo, DbType.String);
                InsertParam.Add("DividendType", (Int32)DividendAccount.DividendType, DbType.Int32);
                InsertParam.Add("Amount", DividendAccount.Amount, DbType.Decimal);
                InsertParam.Add("StartDate", DividendAccount.StartDate, DbType.Date);
                InsertParam.Add("EndDate", DividendAccount.EndDate, DbType.String);
                InsertParam.Add("State", (Int32)DividendAccount.State, DbType.Date);
                InsertParam.Add("Remark", DividendAccount.Remark, DbType.String);
                DividendAccount.Id = await SqlContext.Dapper.ExecuteScalarAsync<Int64>(InsertCity.ToString(), InsertParam);
            }
            #endregion

            SqlContext.Dapper.Open();
            using (IDbTransaction transaction = SqlContext.Dapper.BeginTransaction())
            {
                try
                {
                    Int32 Rows = 0;

                    #region 更新分红账户
                    Rows = SqlContext.Dapper.Execute("UPDATE city_cash_dividend SET Amount = Amount + @Amount WHERE Id = @AccountId;",
                        new { dividend.Amount, AccountId = DividendAccount.Id }, transaction);
                    if (Rows != 1) { throw new Exception("更新现金分红账户 失败"); }
                    #endregion

                    #region 写入分红记录
                    Decimal PreChange = SqlContext.Dapper.QueryFirstOrDefault<Decimal?>("SELECT PostChange FROM city_cash_dividend_record WHERE CityNo = @CityNo ORDER BY Id DESC;",
                        new { dividend.CityNo }, transaction) ?? 0;
                    Decimal PostChange = PreChange + dividend.Amount;

                    StringBuilder DividendRecordSql = new StringBuilder();
                    DividendRecordSql.Append("INSERT INTO `city_cash_dividend_record`(`CityNo`, `ModifyType`, `ModifyDesc`, `PreChange`, `Incurred`, `PostChange`, `CreateTime`) ");
                    DividendRecordSql.Append("VALUES (@CityNo, @ModifyType, @ModifyDesc, @PreChange, @Incurred, @PostChange, NOW());");

                    DynamicParameters DividendRecordParam = new DynamicParameters();
                    DividendRecordParam.Add("CityNo", dividend.CityNo, DbType.String);
                    DividendRecordParam.Add("ModifyType", (Int32)dividend.Type, DbType.Int32);
                    DividendRecordParam.Add("ModifyDesc", String.Join(",", dividend.Desc), DbType.String);
                    DividendRecordParam.Add("PreChange", PreChange, DbType.Decimal);
                    DividendRecordParam.Add("Incurred", dividend.Amount, DbType.Decimal);
                    DividendRecordParam.Add("PostChange", PostChange, DbType.Decimal);

                    Rows = SqlContext.Dapper.Execute(DividendRecordSql.ToString(), DividendRecordParam, transaction);
                    if (Rows != 1) { throw new Exception("写入现金分红记录失败"); }
                    #endregion

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Core.SystemLog.Error($"城市合伙人 ==> Yo帮分红\r\n{dividend.ToString()}", ex);
                }
                finally
                {
                    if (SqlContext.Dapper.State == ConnectionState.Open) { SqlContext.Dapper.Close(); }
                }
            }
        }

        /// <summary>
        /// 糖果分红
        /// </summary>
        /// <param name="dividend"></param>
        /// <returns></returns>
        public async Task CandyDividend(ReqCityDividend dividend)
        {
            #region 获取本期 账户
            CityCashDividend DividendAccount;
            StringBuilder QueryCitySql = new StringBuilder();
            QueryCitySql.Append("SELECT * FROM city_candy_dividend WHERE CityNo = @CityNo AND DividendType = @DividendType AND NOW() BETWEEN StartDate AND EndDate;");
            DividendAccount = await SqlContext.Dapper.QueryFirstOrDefaultAsync<CityCashDividend>(QueryCitySql.ToString(), new { dividend.CityNo, DividendType = (Int32)dividend.Type });
            if (DividendAccount == null)
            {
                DividendAccount = new CityCashDividend()
                {
                    CityNo = dividend.CityNo,
                    DividendType = dividend.Type,
                    Amount = 0,
                    StartDate = GetWeekDate((Int32)dividend.Type).Date,
                    EndDate = GetWeekDate((Int32)dividend.Type).AddDays(7).Date,
                    State = DividendState.Normal,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    Remark = String.Empty
                };
                try
                {
                    StringBuilder InsertCity = new StringBuilder();
                    DynamicParameters InsertParam = new DynamicParameters();
                    InsertCity.Append("INSERT INTO `city_candy_dividend`(`CityNo`, `DividendType`, `Amount`, `StartDate`, `EndDate`, `State`, `Remark`) ");
                    InsertCity.Append("VALUES (@CityNo, @DividendType, @Amount, @StartDate, @EndDate, @State, @Remark);select @@IDENTITY");
                    InsertParam.Add("CityNo", DividendAccount.CityNo, DbType.String);
                    InsertParam.Add("DividendType", (Int32)DividendAccount.DividendType, DbType.Int32);
                    InsertParam.Add("Amount", DividendAccount.Amount, DbType.Decimal);
                    InsertParam.Add("StartDate", DividendAccount.StartDate, DbType.Date);
                    InsertParam.Add("EndDate", DividendAccount.EndDate, DbType.Date);
                    InsertParam.Add("State", (Int32)DividendAccount.State, DbType.Int32);
                    InsertParam.Add("Remark", DividendAccount.Remark, DbType.String);
                    DividendAccount.Id = await SqlContext.Dapper.ExecuteScalarAsync<Int64>(InsertCity.ToString(), InsertParam);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            #endregion
            SqlContext.Dapper.Open();
            using (IDbTransaction transaction = SqlContext.Dapper.BeginTransaction())
            {
                try
                {
                    Int32 Rows = 0;
                    #region 更新分红账户
                    Rows = SqlContext.Dapper.Execute("UPDATE city_candy_dividend SET Amount = Amount + @Amount WHERE Id = @AccountId;",
                        new { dividend.Amount, AccountId = DividendAccount.Id }, transaction);
                    if (Rows != 1) { throw new Exception("更新糖果分红账户 失败"); }
                    #endregion

                    #region 写入分红记录
                    Decimal PreChange = SqlContext.Dapper.QueryFirstOrDefault<Decimal?>("SELECT PostChange FROM city_candy_dividend_record WHERE CityNo = @CityNo ORDER BY Id DESC;",
                        new { dividend.CityNo }, transaction) ?? 0;
                    Decimal PostChange = PreChange + dividend.Amount;

                    StringBuilder DividendRecordSql = new StringBuilder();
                    DividendRecordSql.Append("INSERT INTO `city_candy_dividend_record`(`CityNo`, `ModifyType`, `ModifyDesc`, `PreChange`, `Incurred`, `PostChange`, `CreateTime`) ");
                    DividendRecordSql.Append("VALUES (@CityNo, @ModifyType, @ModifyDesc, @PreChange, @Incurred, @PostChange, NOW());");

                    DynamicParameters DividendRecordParam = new DynamicParameters();
                    DividendRecordParam.Add("CityNo", dividend.CityNo, DbType.String);
                    DividendRecordParam.Add("ModifyType", (Int32)dividend.Type, DbType.Int32);
                    DividendRecordParam.Add("ModifyDesc", String.Join(",", dividend.Desc), DbType.String);
                    DividendRecordParam.Add("PreChange", PreChange, DbType.Decimal);
                    DividendRecordParam.Add("Incurred", dividend.Amount, DbType.Decimal);
                    DividendRecordParam.Add("PostChange", PostChange, DbType.Decimal);

                    Rows = SqlContext.Dapper.Execute(DividendRecordSql.ToString(), DividendRecordParam, transaction);
                    if (Rows != 1) { throw new Exception("写入糖果分红记录失败"); }
                    #endregion

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Core.SystemLog.Error($"城市合伙人 ==> Yo帮分红\r\n{dividend.ToString()}", ex);
                }
                finally
                {
                    if (SqlContext.Dapper.State == ConnectionState.Open) { SqlContext.Dapper.Close(); }
                }
            }
        }

        /// <summary>
        /// 获取星期几
        /// </summary>
        /// <param name="Week"></param>
        /// <returns></returns>
        private static DateTime GetWeekDate(Int32 Week)
        {
            DateTime Dt = DateTime.Now;
            if (Dt == null) { Dt = DateTime.Now; }
            Int32 WeekDay = (Int32)Dt.DayOfWeek;
            if (WeekDay == 0) { WeekDay = 7; }
            Dt = Dt.AddDays(-(WeekDay - Week));
            return Dt;
        }
    }
}
