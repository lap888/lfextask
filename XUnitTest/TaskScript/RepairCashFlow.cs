using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace XUnitTest.TaskScript
{
    public class RepairCashFlow
    {
        private readonly IServiceProvider ServiceProvider;
        public RepairCashFlow()
        {
            CommServiceProvider comm = new CommServiceProvider();
            ServiceProvider = comm.GetServiceProvider();
        }


        [Fact]
        public async Task Repair()
        {
            Yoyo.Entity.SqlContext SqlContext = this.ServiceProvider.GetService<Yoyo.Entity.SqlContext>();

            IEnumerable<Int64> UserIds = await SqlContext.Dapper.QueryAsync<Int64>("SELECT UserId FROM yoyo_city_master;");

            foreach (var item in UserIds)
            {
                WalletModel Wallet = SqlContext.Dapper.QueryFirstOrDefault<WalletModel>("SELECT AccountId, Balance FROM `user_account_wallet` WHERE `UserId` = @UserId;", new { UserId = item });
                if (Wallet == null) { continue; }
                Decimal PostChange = SqlContext.Dapper.QueryFirstOrDefault<Decimal>("SELECT PostChange FROM `user_account_wallet_record` WHERE `AccountId` = @AccountId ORDER BY `RecordId` DESC", new { Wallet.AccountId });
                if (Wallet.Balance == PostChange) { continue; }

                IEnumerable<WalletRecord> records = SqlContext.Dapper.Query<WalletRecord>("SELECT * FROM `user_account_wallet_record` WHERE `AccountId` = @AccountId ORDER BY `RecordId` ASC", new { Wallet.AccountId });

                Decimal PreChange = 0;
                foreach (var record in records)
                {
                    if (record.PreChange != PreChange)
                    {
                        SqlContext.Dapper.Execute("UPDATE `user_account_wallet_record` SET `PreChange` = @PreChange, `PostChange` = @PostChange WHERE `RecordId` = @RecordId",
                            new { RecordId = record.RecordId, PreChange, PostChange = PreChange + record.Incurred });
                        PreChange = PreChange + record.Incurred;
                    }
                    else
                    {
                        PreChange = record.PostChange;
                    }
                }
                continue;
            }



        }
    }



    public class WalletModel
    {
        public Int64 AccountId { get; set; }
        public Decimal Balance { get; set; }
    }

    public class WalletRecord
    {
        public Int64 RecordId { get; set; }
        public Int64 AccountId { get; set; }
        public Decimal PreChange { get; set; }
        public Decimal Incurred { get; set; }
        public Decimal PostChange { get; set; }
    }
}
