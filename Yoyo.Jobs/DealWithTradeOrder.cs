using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Diagnostics;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Options;
using Yoyo.Core.Expand;
using Yoyo.Core;
using Yoyo.Entity.Enums;
using CSRedis;

namespace Yoyo.Jobs
{
    public class DealWithTradeOrder : IJob
    {
        private readonly IServiceProvider ServiceProvider;

        public DealWithTradeOrder(IServiceProvider service)
        {
            this.ServiceProvider = service;
        }

        public async Task<MyResult<object>> ForcePaidCoin(IDbConnection dbConnection, CSRedisClient RedisCache, Stopwatch stopwatch)
        {
            MyResult<object> result = new MyResult<object>();
            CommonMoveWallet commonMoveWallet = new CommonMoveWallet();
            using (IDbTransaction transaction = dbConnection.BeginTransaction())
            {
                try
                {
                    //查询超时订单
                    var orderInfos = await dbConnection.QueryAsync<OrderInfo>($"select id,CoinType,TradeNumber,status,amount,fee,buyerUid,sellerUid from coin_trade where `paidEndTime`< now() and status=3", null, transaction);
                    orderInfos.ToList().ForEach(orderInfo =>
                    {
                        //果皮扣除记录
                        var systemUserId = 1;
                        var TotalCandy = orderInfo.Amount + orderInfo.Fee;
                        //减掉卖家用户的冻结账户中的冻结余额并添加流水
                        commonMoveWallet.ChangeWalletAmount(dbConnection, RedisCache, transaction, true, (long)orderInfo.SellerUid, "USDT(ERC20)", -(decimal)(orderInfo.Amount + orderInfo.Fee), LfexCoinnModifyType.Lf_Sell_Coin, true, Math.Round((decimal)orderInfo.Amount, 4).ToString(), orderInfo.CoinType, Math.Round((decimal)orderInfo.Fee, 4).ToString());
                        //增加买家账户中的余额并添加流水
                        commonMoveWallet.ChangeWalletAmount(dbConnection, RedisCache, transaction, true, (long)orderInfo.BuyerUid, "USDT(ERC20)", (decimal)orderInfo.Amount, LfexCoinnModifyType.Lf_buy_Coin, false, Math.Round((decimal)orderInfo.Amount, 4).ToString(), orderInfo.CoinType);
                        //将手续费划入系统
                        commonMoveWallet.ChangeWalletAmount(dbConnection, RedisCache, transaction, true, systemUserId, "USDT(ERC20)", (decimal)orderInfo.Fee, LfexCoinnModifyType.Lf_Sell_Sys_Fee, false, orderInfo.TradeNumber, orderInfo.Fee.ToString());
                        //更新订单信息
                        dbConnection.Execute($"update coin_trade set status=4 where id = {orderInfo.Id}", null, transaction);
                        transaction.Commit();
                    });
                    if (orderInfos.ToList().Count > 0)
                    {
                        Core.SystemLog.Jobs($"处理超时交易订单 执行完成,关闭{orderInfos.ToList().Count}笔订单,执行时间:{stopwatch.Elapsed.TotalSeconds}秒");
                    }
                    return result;
                }
                catch (System.Exception ex)
                {

                    Core.SystemLog.Jobs($"关闭买方超时订单--强制发放糖果 发生异常", ex);
                    transaction.Rollback();
                    return result;
                }
            }
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var service = this.ServiceProvider.CreateScope())
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                Entity.SqlContext SqlContext = service.ServiceProvider.GetRequiredService<Entity.SqlContext>();
                CSRedis.CSRedisClient RedisCache = service.ServiceProvider.GetRequiredService<CSRedis.CSRedisClient>();
                List<IServices.Utils.SystemUserLevel> SystemLevels = service.ServiceProvider.GetRequiredService<IOptionsMonitor<List<IServices.Utils.SystemUserLevel>>>().CurrentValue;

                using (IDbConnection Db = SqlContext.DapperConnection)
                {
                    Db.Open();
                    try
                    {
                        await ForcePaidCoin(Db, RedisCache, stopwatch);
                    }
                    catch (Exception ex)
                    {
                        Core.SystemLog.Jobs($"关闭买方超时订单--强制发放糖果 发生异常", ex);
                    }
                    finally { if (Db.State == ConnectionState.Open) { Db.Close(); } }
                }
                stopwatch.Stop();
            }
        }

        private string NewGuid20()
        {

            var orderdate = DateTime.Now.ToString("ddHHmmssffffff");
            var ordercode = Guid.NewGuid().GetHashCode();
            var num = 20 - orderdate.Length;
            if (ordercode < 0) { ordercode = -ordercode; }
            var orderlast = ordercode.ToString().Length > num ? ordercode.ToString().Substring(0, num) : ordercode.ToString().PadLeft(num, '0');
            return $"{orderdate}{orderlast}";
        }

        public class AddOrderInfo
        {
            public long UserId { get; set; }

            public string AliPay { get; set; }

            public decimal Amount { get; set; }

            public decimal Price { get; set; }
        }

        public class BuyerLevel
        {
            public long Id { get; set; }

            public string Level { get; set; }
        }

        public class OrderInfo
        {
            public long Id { get; set; }

            public string CoinType { get; set; }

            public string TradeNumber { get; set; }

            public long BuyerUid { get; set; }

            public long SellerUid { get; set; }

            public decimal Amount { get; set; }

            public decimal Fee { get; set; }
        }
    }
}
