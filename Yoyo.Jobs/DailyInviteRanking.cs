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
using CSRedis;
using Yoyo.Entity.Models;
using Yoyo.Core;
using Yoyo.Entity.Enums;

namespace Yoyo.Jobs
{
    public class DailyInviteRanking : IJob
    {
        private readonly String AccountTableName = "user_account_wallet";
        private readonly String RecordTableName = "user_account_wallet_record";
        private readonly String CacheLockKey = "EquityAccount:";

        private readonly IServiceProvider ServiceProvider;


        public DailyInviteRanking(IServiceProvider service)
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
                    var price = 0.071;
                    var priceYB = 0.5;
                    TradeDto model = new TradeDto
                    {
                        Amount = 1,
                        Price = price,
                        CoinType = "糖果"
                    };
                    TradeDto modelYB = new TradeDto
                    {
                        Amount = 1,
                        Price = priceYB,
                        CoinType = "YB"
                    };
                    // if (DateTime.Now.Minute == 1)
                    // {
                    //     model.Price = price;
                    //     model.Amount = 1;
                    // }
                    // if (DateTime.Now.Minute == 2)
                    // {
                    //     model.Price = price - 0.007;
                    //     model.Amount = 1;
                    // }
                    // if (DateTime.Now.Minute == 3)
                    // {
                    //     model.Price = price - 0.003;
                    //     model.Amount = 1;
                    // }
                    // if (DateTime.Now.Minute == 4)
                    // {
                    //     model.Price = price - 0.004;
                    //     model.Amount = 1;
                    // }
                    // if (DateTime.Now.Minute == 5)
                    // {
                    //     model.Price = price - 0.005;
                    //     model.Amount = 1;
                    // }
                    // if (DateTime.Now.Minute == 6)
                    // {
                    //     model.Price = price - 0.006;
                    //     model.Amount = 1;
                    // }
                    // if (DateTime.Now.Minute == 7)
                    // {
                    //     model.Price = price - 0.007;
                    //     model.Amount = 1;
                    // }
                    // if (DateTime.Now.Minute == 8)
                    // {
                    //     model.Price = price - 0.008;
                    //     model.Amount = 1;
                    // }
                    // if (DateTime.Now.Minute == 9)
                    // {
                    //     model.Price = price - 0.009;
                    //     model.Amount = 1;
                    // }
                    // if (DateTime.Now.Minute >= 10 && DateTime.Now.Minute < 20)
                    // {
                    //     model.Price = price;
                    //     model.Amount = 1;
                    // }
                    // if (DateTime.Now.Minute >= 20)
                    // {
                    //     model.Price = price - 0.01;
                    //     model.Amount = 1;
                    // }
                    List<int> userIds = new List<int>() { 5167, 6655, 4971, 4970, 1414, 1414, 4, 4715, 4714, 4713, 4713, 4713 };
                    // string[] nameS6 = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
                    Random ran = new Random();
                    int userId = userIds[ran.Next(0, userIds.Count - 1)];
                    //查询是否挂有订单
                    var hadOrder = IsHadOrder(SqlContext, userId, "糖果");

                    //YB
                    int userIdYB = userIds[ran.Next(0, userIds.Count - 1)];
                    var hadOrderYB = IsHadOrder(SqlContext, userIdYB, "YB");

                    //卡时间
                    if (DateTime.Now.Hour >= 8 && DateTime.Now.Hour < 23)
                    {
                        if (!hadOrder)
                        {
                            if ((DateTime.Now.Minute % 5) == 0)
                            {
                                await StartBuy(SqlContext, model, userId, "tg");
                            }


                        }
                    }
                    //YB
                    if (DateTime.Now.Hour >= 9 && DateTime.Now.Hour < 23)
                    {
                        if (!hadOrderYB)
                        {
                            if ((DateTime.Now.Minute % 6) == 0)
                            {
                                await StartBuy(SqlContext, modelYB, userIdYB, "yb");
                            }
                        }
                    }
                    stopwatch.Stop();
                }
                catch (Exception ex)
                {
                    Core.SystemLog.Jobs("执行挂单完成 发生错误", ex);
                }
            }
        }

        public bool IsHadOrder(Entity.SqlContext SqlContext, int userId, string coinType = "糖果")
        {
            //查询是否挂有订单
            var userInfo = SqlContext.Dapper.QueryFirstOrDefault<User>($"select * from user where id={userId}");
            if (userInfo.Type != 1)
            {
                //
                var orderInfo = SqlContext.Dapper.Query<string>($"SELECT tradeNumber FROM coin_trade WHERE `buyerUid`={userId} and status=1 and coinType='{coinType}';").ToList();
                if (orderInfo.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }


        public async Task StartBuy(Entity.SqlContext SqlContext, TradeDto model, int userId, string flag = "tg")
        {
            //订单信息
            var orderNum = NewGuidN(flag);
            var totalPrice = model.Amount * model.Price;
            //bibi
            var frozenInfo = await FrozenWalletAmount(SqlContext, null, false, userId, (decimal)totalPrice, "USDT(ERC20)");
            if (frozenInfo.Code != 200)
            {
                SystemLog.Debug($"发布买单冻结糖果=={frozenInfo.Message}==userId={userId}");
            }
            else
            {
                //发布订单
                var insertSql = $"insert into coin_trade(tradeNumber,buyerUid,buyerAlipay,amount,price,totalPrice,fee,trendSide,status,coinType)values('{orderNum}',{userId},'118@qq.com',{model.Amount},{model.Price},{totalPrice},0,'BUY',1,'{model.CoinType}');SELECT @@IDENTITY";
                var res = SqlContext.Dapper.ExecuteScalar<long>(insertSql);
            }
        }
        /// <summary>
        /// 20位数 yyyyMMddHHmmss+hashcode
        /// </summary>
        /// <returns></returns>
        public static string NewGuid20()
        {
            var orderdate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var ordercode = Guid.NewGuid().GetHashCode();
            var num = 20 - orderdate.Length;
            if (ordercode < 0) { ordercode = -ordercode; }
            var orderlast = ordercode.ToString().Length > num ? ordercode.ToString().Substring(0, num) : ordercode.ToString().PadLeft(num, '0');
            return $"{orderdate}{orderlast}";
        }
        /// <summary>
        /// 20位数 yyyyMMddHHmmss+hashcode
        /// </summary>
        /// <returns></returns>
        public static string NewGuidN(string str = "YB")
        {
            var ordercode = Guid.NewGuid().ToString("N");
            return $"{str}{ordercode}";
        }


        /// <summary>
        /// 钱包账户余额冻结操作
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public async Task<MyResult<object>> FrozenWalletAmount(Entity.SqlContext SqlContext, IDbTransaction OutTran, bool isUserOutTransaction, long userId, decimal Amount, string coinType)
        {
            MyResult result = new MyResult { Data = false };
            String UpdateSql = $"UPDATE `{AccountTableName}` SET `Frozen`=`Frozen`+{Amount} WHERE `UserId`={userId} AND `CoinType`='{coinType}' AND (`Balance`-`Frozen`)>={Amount} AND (`Frozen`+{Amount})>=0";
            using (var service = this.ServiceProvider.CreateScope())
            {
                CSRedisClient RedisCache = service.ServiceProvider.GetRequiredService<CSRedisClient>();
                CSRedisClientLock CacheLock = null;

                try
                {
                    CacheLock = RedisCache.Lock($"{CacheLockKey}Init_{userId}", 30);
                    if (CacheLock == null) { return result.SetStatus(ErrorCode.InvalidData, "请稍后操作"); }
                    if (isUserOutTransaction)
                    {
                        int Row = await SqlContext.Dapper.ExecuteAsync(UpdateSql, null, OutTran);
                        if (Row != 1) { return result.SetStatus(ErrorCode.InvalidData, $"账户余额{(Amount > 0 ? "冻结" : "解冻")}操作失败"); }
                        result.Data = true;
                        return result;
                    }
                    else
                    {
                        int Row = await SqlContext.Dapper.ExecuteAsync(UpdateSql);
                        if (Row != 1) { return result.SetStatus(ErrorCode.InvalidData, $"账户余额{(Amount > 0 ? "冻结" : "解冻")}操作失败"); }
                        result.Data = true;
                        return result;
                    }

                }
                catch (Exception ex)
                {
                    Yoyo.Core.SystemLog.Debug($"账户余额冻结操作发生错误,\r\n{UpdateSql}", ex);
                    return result.SetStatus(ErrorCode.InvalidData, "发生错误");
                }
                finally
                {
                    if (null != CacheLock) { CacheLock.Unlock(); }
                }
            }
        }

        /// <summary>
        /// 股权账户余额变更
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="Amount"></param>
        /// <param name="useFrozen">使用冻结金额，账户金额增加时，此参数无效</param>
        /// <param name="modifyType">账户变更类型</param>
        /// <param name="Desc">描述</param>
        /// <returns></returns>
        private async Task<Boolean> ChangeWalletAmount(long userId, decimal Amount, Int32 modifyType, bool useFrozen, params string[] Desc)
        {
            if (Amount == 0) { return false; }   //账户无变动，直接返回成功
            if (Amount > 0 && useFrozen) { useFrozen = false; } //账户增加时，无法使用冻结金额

            using (var service = this.ServiceProvider.CreateScope())
            {
                Entity.SqlContext SqlContext = service.ServiceProvider.GetRequiredService<Entity.SqlContext>();
                CSRedisClient RedisCache = service.ServiceProvider.GetRequiredService<CSRedisClient>();

                CSRedisClientLock CacheLock = null;
                UserAccountEquity UserAccount;
                Int64 AccountId;
                String Field = String.Empty, EditSQl = String.Empty, RecordSql = String.Empty, PostChangeSql = String.Empty;
                try
                {
                    CacheLock = RedisCache.Lock($"{CacheLockKey}InitEquity_{userId}", 30);
                    if (CacheLock == null) { return false; }

                    #region 验证账户信息
                    String SelectSql = $"SELECT * FROM `{AccountTableName}` WHERE `UserId` = {userId} LIMIT 1";
                    UserAccount = await SqlContext.Dapper.QueryFirstOrDefaultAsync<UserAccountEquity>(SelectSql);
                    if (UserAccount == null)
                    {
                        String InsertSql = $"INSERT INTO `{AccountTableName}` (`UserId`, `Revenue`, `Expenses`, `Balance`, `Frozen`, `ModifyTime`) VALUES ({userId}, '0', '0', '0', '0', NOW())";
                        Int32 rows = await SqlContext.Dapper.ExecuteAsync(InsertSql);
                        if (rows < 1)
                        {
                            return false;
                        }
                        UserAccount = await SqlContext.Dapper.QueryFirstOrDefaultAsync<UserAccountEquity>(SelectSql);
                    }
                    if (Amount < 0)
                    {
                        if (useFrozen)
                        {
                            if (UserAccount.Frozen < Math.Abs(Amount) || UserAccount.Balance < Math.Abs(Amount)) { return false; }
                        }
                        else
                        {
                            if (UserAccount.Balance < Math.Abs(Amount)) { return false; }
                            if ((UserAccount.Balance - UserAccount.Frozen) < Math.Abs(Amount)) { return false; }
                        }
                    }
                    #endregion

                    AccountId = UserAccount.AccountId;
                    Field = Amount > 0 ? "Revenue" : "Expenses";

                    EditSQl = $"UPDATE `{AccountTableName}` SET `Balance`=`Balance`+{Amount},{(useFrozen ? $"`Frozen`=`Frozen`+{Amount}," : "")}`{Field}`=`{Field}`+{Math.Abs(Amount)},`ModifyTime`=NOW() WHERE `AccountId`={AccountId} {(useFrozen ? $"AND (`Frozen`+{Amount})>=0;" : $"AND(`Balance`-`Frozen`+{Amount}) >= 0;")}";

                    PostChangeSql = $"IFNULL((SELECT `PostChange` FROM `{RecordTableName}` WHERE `AccountId`={AccountId} ORDER BY `RecordId` DESC LIMIT 1),0)";
                    StringBuilder TempRecordSql = new StringBuilder($"INSERT INTO `{RecordTableName}` ");
                    TempRecordSql.Append("( `AccountId`, `PreChange`, `Incurred`, `PostChange`, `ModifyType`, `ModifyDesc`, `ModifyTime` ) ");
                    TempRecordSql.Append($"SELECT {AccountId} AS `AccountId`, ");
                    TempRecordSql.Append($"{PostChangeSql} AS `PreChange`, ");
                    TempRecordSql.Append($"{Amount} AS `Incurred`, ");
                    TempRecordSql.Append($"{PostChangeSql}+{Amount} AS `PostChange`, ");
                    TempRecordSql.Append($"{(int)modifyType} AS `ModifyType`, ");
                    TempRecordSql.Append($"'{String.Join(',', Desc)}' AS `ModifyDesc`, ");
                    TempRecordSql.Append($"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' AS`ModifyTime`");
                    RecordSql = TempRecordSql.ToString();

                    #region 修改账务
                    using (IDbConnection db = SqlContext.DapperConnection)
                    {
                        db.Open();
                        using (IDbTransaction Tran = db.BeginTransaction())
                        {
                            try
                            {
                                Int32 EditRow = db.Execute(EditSQl, null, Tran);
                                Int32 RecordId = db.Execute(RecordSql, null, Tran);
                                if (EditRow == RecordId && EditRow == 1)
                                {
                                    Tran.Commit();
                                    return true;
                                }
                                Tran.Rollback();
                                return false;
                            }
                            catch (Exception ex)
                            {
                                Tran.Rollback();
                                Yoyo.Core.SystemLog.Debug($"股权账户变更发生错误\r\n修改语句：\r\n{EditSQl}\r\n记录语句：{RecordSql}", ex);
                                return false;
                            }
                            finally { if (db.State == ConnectionState.Open) { db.Close(); } }
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    Yoyo.Core.SystemLog.Debug($"股权变更发生错误\r\n修改语句：\r\n{EditSQl}\r\n记录语句：{RecordSql}", ex);
                    return false;
                }
                finally
                {
                    if (null != CacheLock) { CacheLock.Unlock(); }
                }

            }


        }

    }
    public class TradeDto
    {
        /// <summary>
        /// 买单数量
        /// </summary>
        /// <value></value>
        public int Amount { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        /// <value></value>
        public double Price { get; set; }
        /// <summary>
        /// 交易密码
        /// </summary>
        /// <value></value>
        public string TradePwd { get; set; }
        /// <summary>
        /// 支付宝账号
        /// </summary>
        /// <value></value>
        public string Alipay { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNum { get; set; }
        /// <summary>
        /// 币种类 名称
        /// </summary>
        /// <value></value>
        public string CoinType { get; set; }
        /// <summary>
        /// 货比类型 bibi fabi
        /// </summary>
        /// <value></value>
        public string Title { get; set; }
    }
}
