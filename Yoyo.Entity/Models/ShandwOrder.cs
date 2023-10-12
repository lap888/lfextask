using System;
using System.Collections.Generic;
using System.Text;
using Yoyo.Entity.Enums;

namespace Yoyo.Entity.Models
{
    public class ShandwOrder
    {
        public Int64 Id { get; set; }
        public Int64 UserId { get; set; }
        public String ChannelNo { get; set; }
        public String ChannelUid { get; set; }
        public String ChannelOrderNo { get; set; }
        public String GameAppId { get; set; }
        public String Product { get; set; }
        public Decimal PayMoney { get; set; }
        public Decimal Amount { get; set; }
        public DividendState State { get; set; }
        public DateTime CreateTime { get; set; }
        public String Remark { get; set; }
    }
}
