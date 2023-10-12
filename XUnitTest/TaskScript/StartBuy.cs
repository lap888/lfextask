using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Dapper;
using System.Diagnostics;
using Microsoft.Extensions.Options;
using Yoyo.Core.Expand;
using System.Linq;
using System.Data;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Sms.V20190711;
using TencentCloud.Sms.V20190711.Models;
using System.Net.Http;
using System.Text.RegularExpressions;
using TencentCloud.Mps.V20190612.Models;

namespace XUnitTest.TaskScript
{
    public class StartBuy
    {
        private readonly IServiceProvider ServiceProvider;
        public StartBuy()
        {
            CommServiceProvider comm = new CommServiceProvider();
            ServiceProvider = comm.GetServiceProvider();
        }

        [Fact]
        public async Task ShareBon()
        {
            Yoyo.Entity.SqlContext SqlContext = this.ServiceProvider.GetService<Yoyo.Entity.SqlContext>();

            var Shua = 4000;

            for (int i = 0; i < Shua; i++)
            {
                SqlContext.AdClick.Add(new Yoyo.Entity.Models.AdClick
                {
                    UserId = 20,
                    AdId = 0,
                    CandyP = 0,
                    ClickTime = DateTime.Now,
                    ClickDate = DateTime.Now.Date,
                    ClickId = "ShareBon_" + i + "_" + (new Random(Guid.NewGuid().GetHashCode()).Next(1000, 9999))
                });
                SqlContext.SaveChanges();
                await Task.Delay(new Random(Guid.NewGuid().GetHashCode()).Next(1200, 1600));
            }
        }

        [Fact]
        public async Task<List<String>> TestJingDong()
        {
            HttpClient httpClient = this.ServiceProvider.GetService<IHttpClientFactory>().CreateClient();




            //==============请求京东商品详情信息==============//
            var result = await httpClient.GetStringAsync("商品地址{参数}{前端送达}");

            //==============获取商品详情URI列表==============//
            MatchCollection matches = new Regex(@"(?<imgUrl>//([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?)", RegexOptions.IgnoreCase).Matches(result);

            //==============返回前端所有图片列表==============//
            return matches.Where(o => o.Groups["imgUrl"].Value.Contains("jpg") || o.Groups["imgUrl"].Value.Contains("jpeg") || o.Groups["imgUrl"].Value.Contains("png") || o.Groups["imgUrl"].Value.Contains("gif")).Select(o => o.Groups["imgUrl"].Value).ToList();




            // List<string> Urls = new List<string>();

            // // 取得匹配项列表
            // foreach (Match match in matches)
            // {
            //     if (match.Groups["imgUrl"].Value.Contains("jpg", StringComparison.OrdinalIgnoreCase) || match.Groups["imgUrl"].Value.Contains("jpeg", StringComparison.OrdinalIgnoreCase) || match.Groups["imgUrl"].Value.Contains("png", StringComparison.OrdinalIgnoreCase) || match.Groups["imgUrl"].Value.Contains("gif", StringComparison.OrdinalIgnoreCase))
            //     {
            //         Urls.Add(match.Groups["imgUrl"].Value);
            //     }

            // }

        }

        [Fact]
        public async Task QiangDan()
        {
            Yoyo.Entity.SqlContext SqlContext = this.ServiceProvider.GetService<Yoyo.Entity.SqlContext>();


            List<DanInfo> Orders = new List<DanInfo>();

            long SellerUid = 5555;
            string SellerAli = "15373053521";
            decimal Price = 2.50M;
            decimal Amount = 100M;

            while (true)
            {
                while (Orders.Count == 0)
                {
                    Orders = SqlContext.Dapper.Query<DanInfo>($"SELECT id,buyerUid,amount,price FROM `coin_trade` WHERE `status` = '1' AND price>={Price} AND amount>={Amount} AND `buyerUid` NOT IN (0,1,2,3,20,14993,544437)").ToList();
                    await Task.Delay(200);
                }

                foreach (var item in Orders)
                {
                    using (IDbConnection Db = SqlContext.DapperConnection)
                    {
                        Db.Open();
                        IDbTransaction Tran = Db.BeginTransaction();

                        try
                        {
                            var Row = Db.Execute($"update coin_trade set sellerUid = {SellerUid},sellerAlipay='{SellerAli}',fee = 0,dealTime='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}',dealEndTime='{DateTime.Now.AddMinutes(60).ToString("yyyy-MM-dd HH:mm:ss")}', status = 2 where id = {item.Id} AND `status`=1", null, Tran);
                            var Rows = Db.Execute($"update user set `candyNum`=(`candyNum`-{item.Amount}),`freezeCandyNum`=(`freezeCandyNum`+{item.Amount}) where id={SellerUid}  AND candyP>={item.Amount} AND (`candyNum`-{item.Amount})>=0", null, Tran);

                            if (Row != 1) { Tran.Rollback(); continue; }
                            if (Row != Rows) { Tran.Rollback(); continue; }

                            Tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine(ex);
                            Tran.Rollback();
                            continue;
                        }
                        finally { if (Db.State == ConnectionState.Open) { Db.Close(); } }

                        var UserInfo = await SqlContext.User.FirstOrDefaultAsync(o => o.Id == item.BuyerUid);

                        Credential cred = new Credential
                        {
                            SecretId = "AKIDtcu0qRQVgeLrBHPZovpV1RdbNyLWrQ4W",
                            SecretKey = "X6TbtxnPp3qb8QUSmzyOmws0s8qNddSB"
                        };

                        ClientProfile clientProfile = new ClientProfile();
                        HttpProfile httpProfile = new HttpProfile();
                        httpProfile.Endpoint = ("sms.ap-beijing.tencentcloudapi.com");
                        clientProfile.HttpProfile = httpProfile;

                        SmsClient client = new SmsClient(cred, "ap-beijing", clientProfile);
                        SendSmsRequest req = new SendSmsRequest()
                        {
                            PhoneNumberSet = new String[] { "+86" + UserInfo.Mobile },
                            Sign = "哟哟吧",
                            TemplateID = "568502",
                            SmsSdkAppid = "1400343456"
                        };
                        SendSmsResponse resp = client.SendSmsSync(req);

                    }

                }

                Orders = new List<DanInfo>();
            }


            //var Records = SqlContext.Dapper.Query<dynamic>("SELECT * FROM `user_account_wallet_record` WHERE `ModifyType` = '5' ");

            //foreach (var item in Records)
            //{
            //    ChangeWalletAmount(SqlContext.DapperConnection,item.AccountId,-item.Incurred,"分红扣回","0.00");
            //}

        }

        private void ChangeWalletAmount(IDbConnection DataBase, long AccountId, decimal Amount, params string[] Desc)
        {
            String EditSQl, RecordSql, PostChangeSql;

            EditSQl = $"UPDATE `user_account_wallet` SET `Balance`=`Balance`+{Amount},`Expenses`=`Expenses`+{Math.Abs(Amount)},`ModifyTime`=NOW() WHERE `AccountId`={AccountId}";

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
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex);
                    Tran.Rollback();
                }
                finally { if (db.State == ConnectionState.Open) { db.Close(); } }
            }
            #endregion
        }

        public class DanInfo
        {
            public long Id { get; set; }
            public long BuyerUid { get; set; }
            public decimal Amount { get; set; }
            public decimal Price { get; set; }
        }

        #region 小号转移
        [Fact]
        public async Task SmallTransfer()
        {
            List<String> UserPhones = new List<string>
            {
                "13807567773",  //皮皮虾
                "13003405264",  //大肥狼
                "18871774420",    //加蛋
                "13000668618",    //炎炎
                "13360781002",
                "13400756308",
                "18024647643",
                "18001287136",
                "13001687824",
                "13001340862",
                "18922175615",
                "18024006006",
                "18900007538",
                "18900884318"
            };
            String UserSql = $"SELECT id,`mobile`,`name`,`candyNum`,`candyP`,`level`,`inviterMobile` FROM `user` WHERE `mobile` IN ({String.Join(",", UserPhones.ToArray())})";

            Yoyo.Entity.SqlContext SqlContext = this.ServiceProvider.GetService<Yoyo.Entity.SqlContext>();

            var Users = await SqlContext.Dapper.QueryAsync<Yoyo.Entity.Models.User>(UserSql);
            String TransferDesc = "转账";
            Transfer ee = new Transfer { Phone = "17798132156" };   //转入方信息
            var eeUser = await SqlContext.User.FirstOrDefaultAsync(o => o.Mobile == ee.Phone);

            foreach (var item in Users)
            {
                if (item.CandyNum.Value <= 0) { continue; }
                Transfer or = new Transfer
                {
                    Phone = item.Mobile,      //转出方账号
                    CandyNum = item.CandyNum.Value,           //转出糖果数量
                    CandyP = item.CandyP                  //转出果皮数量[TransferType==FALSE 时生效]
                };
                var orUser = item;

                #region 开始系统划拨
                //基础SQL语句
                String UserBase = "UPDATE `user` SET candyNum=candyNum+{0},candyP=candyP+{1},utime=NOW() WHERE id={2};";
                String CandyPBase = "INSERT INTO `user_candyp` (`userId`,`candyP`,`content`,`source`,`createdAt`,`updatedAt`) VALUES ({0},{1},'{2}',{3},NOW(),NOW());";
                String CandyNumBase = "INSERT INTO `gem_records` (`userId`,`num`,`description`,`gemSource`) VALUES ({0},{1},'{2}',{3});";
                using (IDbConnection Db = SqlContext.DapperConnection)
                {
                    Db.Open();
                    IDbTransaction Tran = Db.BeginTransaction();
                    try
                    {
                        //====构建交易SQL语句
                        StringBuilder TradeSql = new StringBuilder();
                        //扣除转出方糖果和果皮，并增加交易记录
                        TradeSql.AppendLine(String.Format(UserBase, -or.CandyNum, -or.CandyP, orUser.Id));
                        if (or.CandyP > 0)
                        {
                            TradeSql.AppendLine(String.Format(CandyPBase, orUser.Id, -or.CandyP, $"系统划拨:{TransferDesc}【{or.CandyNum}】糖果至用户【{ee.Phone}】，扣除【{or.CandyP}】果皮", 4));
                        }
                        TradeSql.AppendLine(String.Format(CandyNumBase, orUser.Id, -or.CandyNum, $"系统划拨:{TransferDesc}【{or.CandyNum}】糖果至用户【{ee.Phone}】", 3));
                        //增加转入方糖果和果皮，并增加交易记录
                        TradeSql.AppendLine(String.Format(UserBase, or.CandyNum, or.CandyP, eeUser.Id));
                        if (or.CandyP > 0)
                        {
                            TradeSql.AppendLine(String.Format(CandyPBase, eeUser.Id, or.CandyP, $"系统划拨:{TransferDesc}【{or.CandyNum}】糖果，赠送【{or.CandyP}】果皮", 4));
                        }
                        TradeSql.AppendLine(String.Format(CandyNumBase, eeUser.Id, or.CandyNum, $"系统划拨:{TransferDesc}【{or.CandyNum}】糖果", 5));

                        String ActionSql = TradeSql.ToString();
                        await Db.ExecuteAsync(ActionSql, null, Tran);
                        Tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine(ex);
                        Tran.Rollback();
                    }
                    finally { if (Db.State == ConnectionState.Open) { Db.Close(); } }
                }
                #endregion
            }

        }
        #endregion

        #region 城主扣除糖果
        [Fact]
        public void CityCandy()
        {
            Yoyo.Entity.SqlContext SqlContext = this.ServiceProvider.GetService<Yoyo.Entity.SqlContext>();

            List<long> Citys = SqlContext.Dapper.Query<long>($"SELECT UserId FROM `yoyo_city_master` ").ToList();

            foreach (var item in Citys)
            {
                String UserBase = "UPDATE `user` SET candyNum=candyNum+{0},candyP=candyP+{1},utime=NOW() WHERE id={2};";
                String CandyNumBase = "INSERT INTO `gem_records` (`userId`,`num`,`description`,`gemSource`) VALUES ({0},{1},'{2}',{3});";
                using (IDbConnection Db = SqlContext.DapperConnection)
                {
                    Db.Open();
                    IDbTransaction Tran = Db.BeginTransaction();
                    try
                    {
                        var UserId = item;
                        if (UserId == 707963) { UserId = 20871; }
                        if (UserId == 472669) { UserId = 42240; }
                        //====构建交易SQL语句
                        StringBuilder TradeSql = new StringBuilder();
                        //扣除转出方糖果和果皮，并增加交易记录
                        TradeSql.AppendLine(String.Format(UserBase, -5000, 0, UserId));
                        TradeSql.AppendLine(String.Format(CandyNumBase, UserId, -5000, $"系统销毁:城市合伙人销毁5000糖果", 3));
                        String ActionSql = TradeSql.ToString();
                        Db.Execute(ActionSql, null, Tran, 100);
                        Tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine(ex);
                        Tran.Rollback();
                    }
                    finally { if (Db.State == ConnectionState.Open) { Db.Close(); } }
                }
            }
        }
        #endregion


        #region 系统划拨
        /// <summary>
        /// 系统划拨
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SystemTransfer()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //以下信息仔细核对[系统账号: 11811811888 ]
            Boolean AddDevote = false;   //是否给予活跃贡献值以及系统任务和系统活动
            Boolean TransferType = false;   //划拨类型： FALSE---->指定划拨,TRUE---->计算划拨[果皮根据转入方等级计算]
            String TransferDesc = "平台代收";   //划拨描述
            Decimal Price = 2.6M;
            Transfer or = new Transfer
            {
                Phone = "13393113521",      //转出方账号
                CandyNum = 40000M,           //转出糖果数量
                CandyP = 0M                  //转出果皮数量[TransferType==FALSE 时生效]
            };
            Transfer ee = new Transfer { Phone = "13731151001" };   //转入方信息

            #region 执行划拨方法
            Yoyo.Entity.SqlContext SqlContext = this.ServiceProvider.GetService<Yoyo.Entity.SqlContext>();
            Yoyo.IServices.IMember.ISubscribe subscribe = this.ServiceProvider.GetService<Yoyo.IServices.IMember.ISubscribe>();
            List<Yoyo.IServices.Utils.SystemUserLevel> Settings = this.ServiceProvider.GetRequiredService<IOptionsMonitor<List<Yoyo.IServices.Utils.SystemUserLevel>>>().CurrentValue;

            var orUser = await SqlContext.User.FirstOrDefaultAsync(o => o.Mobile == or.Phone);
            var eeUser = await SqlContext.User.FirstOrDefaultAsync(o => o.Mobile == ee.Phone);

            if (TransferType)
            {
                var SysLevel = Settings.FirstOrDefault(o => o.Level.ToLower().Equals(eeUser.Level.ToLower()));
                or.CandyP = or.CandyNum * SysLevel.BuyRate;
            }

            if (or.CandyP <= 0 && or.CandyNum <= 0) { return; }

            #region 开始系统划拨
            //基础SQL语句
            String UserBase = "UPDATE `user` SET candyNum=candyNum+{0},candyP=candyP+{1},utime=NOW() WHERE id={2};";
            String CandyPBase = "INSERT INTO `user_candyp` (`userId`,`candyP`,`content`,`source`,`createdAt`,`updatedAt`) VALUES ({0},{1},'{2}',{3},NOW(),NOW());";
            String CandyNumBase = "INSERT INTO `gem_records` (`userId`,`num`,`description`,`gemSource`) VALUES ({0},{1},'{2}',{3});";
            using (IDbConnection Db = SqlContext.DapperConnection)
            {
                Db.Open();
                IDbTransaction Tran = Db.BeginTransaction();
                try
                {
                    //====构建交易SQL语句
                    StringBuilder TradeSql = new StringBuilder();
                    //扣除转出方糖果和果皮，并增加交易记录
                    TradeSql.AppendLine(String.Format(UserBase, -or.CandyNum, -or.CandyP, orUser.Id));
                    if (or.CandyP > 0)
                    {
                        TradeSql.AppendLine(String.Format(CandyPBase, orUser.Id, -or.CandyP, $"系统划拨:{TransferDesc}【{or.CandyNum}】糖果至用户【{ee.Phone}】，扣除【{or.CandyP}】果皮", 4));
                    }
                    TradeSql.AppendLine(String.Format(CandyNumBase, orUser.Id, -or.CandyNum, $"系统划拨:{TransferDesc}【{or.CandyNum}】糖果至用户【{ee.Phone}】", 3));
                    //增加转入方糖果和果皮，并增加交易记录
                    TradeSql.AppendLine(String.Format(UserBase, or.CandyNum, or.CandyP, eeUser.Id));
                    if (or.CandyP > 0)
                    {
                        TradeSql.AppendLine(String.Format(CandyPBase, eeUser.Id, or.CandyP, $"系统划拨:{TransferDesc}【{or.CandyNum}】糖果，赠送【{or.CandyP}】果皮", 4));
                    }
                    TradeSql.AppendLine(String.Format(CandyNumBase, eeUser.Id, or.CandyNum, $"系统划拨:{TransferDesc}【{or.CandyNum}】糖果", 5));
                    if (AddDevote)
                    {
                        var orderNum = NewGuid20();
                        TradeSql.AppendLine($"INSERT INTO `coin_trade` (`tradeNumber`, `buyerUid`, `buyerAlipay`, `sellerUid`, `sellerAlipay`, `amount`, `price`, `totalPrice`, `trendSide`, `pictureUrl`, `status`, `entryOrderTime`, `paidTime`, `payCoinTime`, `dealTime`, `finishTime`, `ctime`, `utime`, `fee`, `appealTime`, `paidEndTime`, `dealEndTime`, `buyerBan`, `sellerBan`, `monthlyTradeCount`) VALUES ('{orderNum}', {eeUser.Id}, '{eeUser.Alipay}', {orUser.Id}, '{orUser.Alipay}', {or.CandyNum}, {Price}, {or.CandyNum * Price}, 'BUY', NULL, 4, NOW(), NOW(), NULL, NOW(), NULL, NOW(), NOW(), 0, NULL, NOW(), NOW(), 0, 0, NULL);");
                    }
                    String ActionSql = TradeSql.ToString();
                    Db.Execute(ActionSql, null, Tran, 100);
                    Tran.Commit();

                    if (AddDevote)
                    {
                        await subscribe.SubscribeTask((new Yoyo.IServices.Utils.DailyTask
                        {
                            UserId = eeUser.Id,
                            TaskType = Yoyo.IServices.Utils.TaskType.BUYER,
                            Devote = or.CandyNum,
                            CarryOut = 1
                        }).ToJson());
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex);
                    Tran.Rollback();
                }
                finally { if (Db.State == ConnectionState.Open) { Db.Close(); } }
            }
            #endregion

            #endregion

            stopwatch.Stop();
            var TotalTime = stopwatch.Elapsed.TotalMinutes;
        }

        public class Transfer
        {
            public String Phone { get; set; }

            public Decimal CandyNum { get; set; }

            public Decimal CandyP { get; set; }
        }
        #endregion

        #region 发布系统买单
        /// <summary>
        /// 发布系统买单
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddOrder()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Yoyo.Entity.SqlContext SqlContext = ServiceProvider.GetService<Yoyo.Entity.SqlContext>();


            //18779332081-------------扶桑--------------ID:438934
            //18730303795-------------灵魂--------------ID:4
            //15373053521-------------馍馍干--------------ID:686600
            //18333103619-------------鸟窝--------------ID:2
            //15928669826-------------火火--------------ID:20871
            //18850549753-------------龙行--------------ID:2625
            //13860767417-------------老爸--------------ID:24447
            //17336318815-------------悟道--------------ID:20


            int AddTotal = 20;
            OrderInfo Order = new OrderInfo
            {
                UserId = 3134,     //用户ID
                Amount = 10,     //购买数量
                Price = 1.5M,    //单价
                AliPay = "13663112276"     //用户支付宝
            };

            for (int i = 0; i < AddTotal; i++)
            {
                //bool nohave = false;
                //while (!nohave)
                //{
                //    nohave = SqlContext.Dapper.ExecuteScalar<int>($"SELECT COUNT(1) from coin_trade WHERE buyerUid=1 and `status`=1") < 5;
                //    if (!nohave) { await Task.Delay(5000); }
                //}
                try
                {
                    var orderNum = NewGuid20();
                    var insertSql = $"insert into coin_trade(tradeNumber,buyerUid,buyerAlipay,amount,price,totalPrice,fee,trendSide,status)values('{orderNum}',{Order.UserId},'{Order.AliPay}',{Order.Amount},{Order.Price},{Order.Amount * Order.Price},0,'BUY',1);SELECT @@IDENTITY";
                    var res = await SqlContext.Dapper.ExecuteScalarAsync<long>(insertSql);
                    if (res < 1) { i -= 1; }
                    await SqlContext.Dapper.ExecuteAsync($"INSERT INTO `coin_trade_location` (`TradeId`, `Buyer_Location_X`, `Buyer_Location_Y`, `Buyer_Province`, `Buyer_City`, `Buyer_Area`) VALUES (@TradeId, @LocationX, @LocationY, @Province, @City, @Area)", new { TradeId = res, LocationX = 0, LocationY = 0, Province = "", City = "", Area = "" });
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex);
                    i -= 1;
                }
            }

            stopwatch.Stop();
            var TotalTime = stopwatch.Elapsed.TotalMinutes;

        }

        public class OrderInfo
        {
            public long UserId { get; set; }

            public string AliPay { get; set; }

            public decimal Amount { get; set; }

            public decimal Price { get; set; }
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
        #endregion
    }
}
