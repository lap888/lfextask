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
using static Yoyo.Service.Member.Subscribe;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Yoyo.Jobs
{
    public class SellCoin : IJob
    {
        private readonly String AccountTableName = "user_account_wallet";
        private readonly String RecordTableName = "user_account_wallet_record";
        private readonly String CacheLockKey = "EquityAccount:";

        private readonly IServiceProvider ServiceProvider;
        private readonly HttpClient Client;


        //匹配买单
        public SellCoin(IHttpClientFactory factory, IServiceProvider service)
        {
            this.ServiceProvider = service;

            Client = factory.CreateClient("JPushSMS");
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var service = this.ServiceProvider.CreateScope())
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                //查询符合条件订单
                Entity.SqlContext SqlContext = service.ServiceProvider.GetRequiredService<Entity.SqlContext>();
                var sql1 = $"SELECT tradeNumber FROM coin_trade WHERE buyerAlipay='118@qq.com' and status=1 and price>0.07 order by price desc;";
                var order = SqlContext.Dapper.Query<string>(sql1).ToList();
                var sql2 = $"SELECT tradeNumber FROM coin_trade WHERE buyerAlipay<>'118@qq.com' and status=1 and price>0.055 and amount<10000 and coinType='糖果' order by price desc;";
                var realOrder = SqlContext.Dapper.Query<string>(sql2).ToList();

                //YB
                var sqlYB = $"SELECT tradeNumber FROM coin_trade WHERE buyerAlipay='118@qq.com' and status=1 order by price desc;";
                var orderYB = SqlContext.Dapper.Query<string>(sqlYB).ToList();

                var sqlFabi = $"select a.`tradeNumber` from (SELECT tradeNumber,`buyerUid` FROM coin_trade WHERE buyerUid is not NULL and status=1 and price>6.3 and amount<100 and coinType='USDT(ERC20)' order by price desc) a left join user u on a.`buyerUid`=u.id where u.type<>1;";
                var sqlFabiOrder = SqlContext.Dapper.Query<string>(sqlFabi).ToList();
                try
                {
                    Random random = new Random();
                    if (order.Count > 0)
                    {
                        if (DateTime.Now.Hour >= 9 && DateTime.Now.Hour < 23)
                        {
                            // var number = order[random.Next(0, order.Count - 1)];
                            var number = order[0];
                            TradeDto model = new TradeDto
                            {
                                OrderNum = number,
                                CoinType = "糖果",
                                Amount = 1
                            };
                            var res = await StartSell("bibi", SqlContext, model, 2);
                        }
                    }
                    //YB
                    if (orderYB.Count > 0)
                    {
                        if (DateTime.Now.Hour >= 9 && DateTime.Now.Hour < 23)
                        {
                            var number = orderYB[0];
                            TradeDto model = new TradeDto
                            {
                                OrderNum = number,
                                CoinType = "YB",
                                Amount = 1
                            };
                            await StartSell("bibi", SqlContext, model, 2);
                        }
                    }
                    //查询符合条件订单
                    if (realOrder.Count > 0)
                    {
                        if (DateTime.Now.Hour >= 9 && DateTime.Now.Hour < 23)
                        {
                            var number1 = realOrder[random.Next(0, realOrder.Count - 1)];
                            TradeDto model1 = new TradeDto
                            {
                                OrderNum = number1,
                                CoinType = "糖果",
                                Amount = 1
                            };
                            var res1 = await StartSell("bibi", SqlContext, model1, 1414);
                        }
                    }
                    //匹配法币
                    if (DateTime.Now.Hour >= 9 && DateTime.Now.Hour <= 22)
                    {
                        if (sqlFabiOrder.Count > 0)
                        {
                            var number2 = sqlFabiOrder[random.Next(0, sqlFabiOrder.Count - 1)];
                            TradeDto model1 = new TradeDto
                            {
                                OrderNum = number2,
                                CoinType = "USDT(ERC20)",
                                Amount = 1,
                                Alipay = "17336318815"
                            };
                            var res1 = await StartSell("fabi", SqlContext, model1, 1414);
                        }
                    }
                    stopwatch.Stop();
                }
                catch (Exception ex)
                {
                    Core.SystemLog.Jobs($"匹配买单完成 发生错误 == sql1={sql1}\r\n==sql2={sql2}\r\n ==", ex);
                }
            }


        }


        public async Task<MyResult<object>> StartSell(string title, Entity.SqlContext SqlContext, TradeDto model, int userId)
        {
            MyResult result = new MyResult();
            //查订单是否存在
            var order = SqlContext.Dapper.QueryFirstOrDefault<CoinTrade>($"SELECT `status`,coinType, buyerUid, amount, totalPrice, trendSide FROM coin_trade WHERE tradeNumber='{model.OrderNum}';");
            if (order == null) { return result.SetStatus(ErrorCode.SystemError, "此订单已经被别人抢单..."); }
            if (order.Status != 1) { return result.SetStatus(ErrorCode.SystemError, "此订单已经被别人抢单..."); }
            if (!order.TrendSide.Equals("BUY", StringComparison.OrdinalIgnoreCase)) { return result.SetStatus(ErrorCode.SystemError, "订单类型错误"); }
            decimal fee = 0;
            decimal SellRate = 0.01M;
            fee = order.Amount * SellRate;
            //查余额
            var coinBalance = SqlContext.Dapper.QueryFirstOrDefault<decimal>($"select `Balance` from `user_account_wallet` where `CoinType`='{model.CoinType}' and userId={userId}");
            Decimal TotalCandy = fee + order.Amount;
            if (coinBalance < TotalCandy)
            {
                return result.SetStatus(ErrorCode.InvalidData, $"您当前{model.CoinType}为{coinBalance},不足以出售{model.Amount}个{model.CoinType}");
            }
            //买家和卖家不能相同
            if (order.BuyerUid == userId)
            {
                return result.SetStatus(ErrorCode.SystemError, "自己无法卖给自己!");
            }
            if (SqlContext.Dapper.State == ConnectionState.Closed) { SqlContext.Dapper.Open(); }
            using (IDbTransaction transaction = SqlContext.Dapper.BeginTransaction())
            {
                try
                {
                    if (title == "bibi")
                    {
                        await SqlContext.Dapper.ExecuteAsync($"update coin_trade set sellerUid = {userId},sellerAlipay='118@gmail.com',fee = {fee},dealTime='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}',dealEndTime='{DateTime.Now.AddMinutes(60).ToString("yyyy-MM-dd HH:mm:ss")}', status = 4 where tradeNumber='{model.OrderNum}'", null, transaction);

                        //卖方扣币
                        var subSellerCoin = await ChangeWalletAmount(SqlContext, transaction, true, userId, model.CoinType, -Math.Round(TotalCandy, 4), LfexCoinnModifyType.Lf_Sell_Coin, false, Math.Round(order.Amount, 4).ToString(), model.CoinType, Math.Round(fee, 4).ToString());
                        if (subSellerCoin.Code != 200)
                        {
                            transaction.Rollback();
                            return result.SetStatus(ErrorCode.SystemError, subSellerCoin.Message);
                        }
                        //买方扣U
                        var subBuyerCoin = await ChangeWalletAmount(SqlContext, transaction, true, (long)order.BuyerUid, "USDT(ERC20)", -(Math.Round((decimal)order.TotalPrice, 4)), LfexCoinnModifyType.Lf_buy_Coin_Sub_Usdt, true, Math.Round(order.Amount, 4).ToString(), model.CoinType, Math.Round((decimal)order.TotalPrice, 4).ToString());
                        if (subBuyerCoin.Code != 200)
                        {
                            transaction.Rollback();
                            return result.SetStatus(ErrorCode.SystemError, subBuyerCoin.Message);
                        }
                        //买方加币
                        var addBuyerCoin = await ChangeWalletAmount(SqlContext, transaction, true, (long)order.BuyerUid, model.CoinType, Math.Round(order.Amount, 4), LfexCoinnModifyType.Lf_buy_Coin, false, Math.Round(order.Amount, 4).ToString(), model.CoinType);
                        if (addBuyerCoin.Code != 200)
                        {
                            transaction.Rollback();
                            return result.SetStatus(ErrorCode.SystemError, addBuyerCoin.Message);
                        }
                        //卖方加U
                        var subSellerUsdtCoin = await ChangeWalletAmount(SqlContext, transaction, true, userId, "USDT(ERC20)", Math.Round((decimal)order.TotalPrice, 4), LfexCoinnModifyType.Lf_Sell_Coin_Add_Usdt, false, Math.Round(order.Amount, 4).ToString(), model.CoinType, Math.Round((decimal)order.TotalPrice, 4).ToString());
                        if (subSellerUsdtCoin.Code != 200)
                        {
                            transaction.Rollback();
                            return result.SetStatus(ErrorCode.SystemError, subSellerUsdtCoin.Message);
                        }
                    }
                    else
                    {
                        var res1 = await FrozenWalletAmount(SqlContext, transaction, true, userId, TotalCandy, "USDT(ERC20)");
                        if (res1.Code != 200)
                        {
                            return result.SetStatus(ErrorCode.SystemError, res1.Message);
                        }

                        SqlContext.Dapper.Execute($"update coin_trade set sellerUid = {userId},sellerAlipay='{model.Alipay}',fee = {fee},dealTime='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}',dealEndTime='{DateTime.Now.AddMinutes(60).ToString("yyyy-MM-dd HH:mm:ss")}', status = 2 where tradeNumber = '{model.OrderNum}'", null, transaction);

                        //我的信息记录
                        var msg = $"你发布的{Math.Round((decimal)order.Amount, 4)}{model.CoinType}买单已被接单，请到“{model.CoinType}”-“订单”-“交易中” 查看卖家支付宝，并按买单中显示的金额付款，上传付款截图";
                        SqlContext.Dapper.Execute($"insert into notice_infos (userId, content, refId, type,title) values ({order.BuyerUid}, '{msg}', '{model.OrderNum}', '1','买{model.CoinType}通知')", null, transaction);
                        var buyerMobile = SqlContext.Dapper.QueryFirstOrDefault<string>($"select mobile from user where id={order.BuyerUid}");
                        //短信通知
                        await CommonSendToBuyer(buyerMobile);
                    }

                    transaction.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return result.SetStatus(ErrorCode.InvalidData, $"Error{ex}");
                }
                finally { if (SqlContext.Dapper.State == ConnectionState.Open) { SqlContext.Dapper.Close(); } }
            }
        }

        /// <summary>
        /// 通知买方 CommonSendToBuyer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<MyResult<MsgDto>> CommonSendToBuyer(string mobile)
        {
            MyResult<MsgDto> result = new MyResult<MsgDto>();
            var reqJson = JsonConvert.SerializeObject(new { mobile = mobile, temp_id = "184449" });
            StringContent content = new StringContent(reqJson);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await this.Client.PostAsync("https://api.sms.jpush.cn/v1/messages", content);
            String res = await response.Content.ReadAsStringAsync();
            result.Data = JsonConvert.DeserializeObject<MsgDto>(res);//res.GetModel<MsgDto>();            
            return result;
        }

        /// <summary>
        /// Coin钱包账户余额变更 common
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="Amount"></param>
        /// <param name="useFrozen">使用冻结金额，账户金额增加时，此参数无效</param>
        /// <param name="modifyType">账户变更类型</param>
        /// <param name="Desc">描述</param>
        /// <returns></returns>
        public async Task<MyResult<object>> ChangeWalletAmount(Entity.SqlContext SqlContext, IDbTransaction OutTran, bool isUserOutTransaction, long userId, string coinType, decimal Amount, LfexCoinnModifyType modifyType, bool useFrozen, params string[] Desc)
        {
            MyResult result = new MyResult { Data = false };
            if (Amount == 0) { return new MyResult { Data = true }; }   //账户无变动，直接返回成功
            if (Amount > 0 && useFrozen) { useFrozen = false; } //账户增加时，无法使用冻结金额
            using (var service = this.ServiceProvider.CreateScope())
            {
                CSRedisClient RedisCache = service.ServiceProvider.GetRequiredService<CSRedisClient>();
                CSRedisClientLock CacheLock = null;
                UserAccountWallet UserAccount;
                Int64 AccountId;
                String Field = String.Empty, EditSQl = String.Empty, RecordSql = String.Empty, PostChangeSql = String.Empty;
                try
                {
                    CacheLock = RedisCache.Lock($"{CacheLockKey}Init_{userId}", 30);
                    if (CacheLock == null) { return result.SetStatus(ErrorCode.InvalidData, "请稍后操作"); }

                    #region 验证账户信息
                    String SelectSql = $"SELECT * FROM `{AccountTableName}` WHERE `UserId` = {userId} AND `CoinType`='{coinType}' LIMIT 1";
                    if (isUserOutTransaction)
                    {
                        UserAccount = await SqlContext.Dapper.QueryFirstOrDefaultAsync<UserAccountWallet>(SelectSql, null, OutTran);
                    }
                    else
                    {
                        UserAccount = await SqlContext.Dapper.QueryFirstOrDefaultAsync<UserAccountWallet>(SelectSql);
                    }
                    if (UserAccount == null) { return result.SetStatus(ErrorCode.InvalidData, "账户不存在"); }
                    if (Amount < 0)
                    {
                        if (useFrozen)
                        {
                            if (UserAccount.Frozen < Math.Abs(Amount) || UserAccount.Balance < Math.Abs(Amount)) { return result.SetStatus(ErrorCode.InvalidData, "账户余额不足[F]"); }
                        }
                        else
                        {
                            if ((UserAccount.Balance - UserAccount.Frozen) < Math.Abs(Amount)) { return result.SetStatus(ErrorCode.InvalidData, "账户余额不足[B]"); }
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
                    if (SqlContext.Dapper.State == ConnectionState.Closed) { SqlContext.Dapper.Open(); }
                    if (isUserOutTransaction)
                    {
                        IDbTransaction Tran = OutTran;
                        try
                        {
                            Int32 EditRow = SqlContext.Dapper.Execute(EditSQl, null, Tran);
                            Int32 RecordId = SqlContext.Dapper.Execute(RecordSql, null, Tran);
                            if (EditRow == RecordId && EditRow == 1)
                            {
                                if (!isUserOutTransaction)
                                {
                                    Tran.Commit();
                                }
                                return new MyResult { Data = true };
                            }
                            Tran.Rollback();
                            return result.SetStatus(ErrorCode.InvalidData, "账户变更发生错误");
                        }
                        catch (Exception ex)
                        {
                            Tran.Rollback();
                            Yoyo.Core.SystemLog.Debug($"钱包账户余额变更发生错误\r\n修改语句：\r\n{EditSQl}\r\n记录语句：{RecordSql}", ex);
                            return result.SetStatus(ErrorCode.InvalidData, "发生错误");
                        }
                        finally
                        {
                            if (!isUserOutTransaction)
                            {
                                if (SqlContext.Dapper.State == ConnectionState.Open) { SqlContext.Dapper.Close(); }
                            }
                        }
                    }
                    else
                    {
                        using (IDbTransaction Tran = SqlContext.Dapper.BeginTransaction())
                        {
                            try
                            {
                                Int32 EditRow = SqlContext.Dapper.Execute(EditSQl, null, Tran);
                                Int32 RecordId = SqlContext.Dapper.Execute(RecordSql, null, Tran);
                                if (EditRow == RecordId && EditRow == 1)
                                {
                                    if (!isUserOutTransaction)
                                    {
                                        Tran.Commit();
                                    }
                                    return new MyResult { Data = true };
                                }
                                Tran.Rollback();
                                return result.SetStatus(ErrorCode.InvalidData, "账户变更发生错误");
                            }
                            catch (Exception ex)
                            {
                                Tran.Rollback();
                                Yoyo.Core.SystemLog.Debug($"钱包账户余额变更发生错误\r\n修改语句：\r\n{EditSQl}\r\n记录语句：{RecordSql}", ex);
                                return result.SetStatus(ErrorCode.InvalidData, "发生错误");
                            }
                            finally
                            {
                                if (!isUserOutTransaction)
                                {
                                    if (SqlContext.Dapper.State == ConnectionState.Open) { SqlContext.Dapper.Close(); }
                                }
                            }
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    Yoyo.Core.SystemLog.Debug($"钱包账户余额变更发生错误\r\n修改语句：\r\n{EditSQl}\r\n记录语句：{RecordSql}", ex);
                    return result.SetStatus(ErrorCode.InvalidData, "发生错误");
                }
                finally
                {
                    if (null != CacheLock) { CacheLock.Unlock(); }
                }
            }
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


    }
    public class MsgDto
    {
        public string Msg_Id { get; set; }
        public bool Is_Valid { get; set; }
        public ErrorDto Error { get; set; }
    }
    public class ErrorDto
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
