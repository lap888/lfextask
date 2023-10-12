using System;
using System.Collections.Generic;

namespace Yoyo.Entity.Models
{
    public partial class User
    {
        public long Id { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public int? AuditState { get; set; }
        public string Name { get; set; }
        public string Rcode { get; set; }
        public string ContryCode { get; set; }
        public string Mobile { get; set; }
        public decimal? CandyNum { get; set; }
        public decimal CandyP { get; set; }
        public decimal? FreezeCandyNum { get; set; }
        public DateTime? CnadyDoAt { get; set; }
        public string InviterMobile { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime Ctime { get; set; }
        public DateTime Utime { get; set; }
        public string AvatarUrl { get; set; }
        public int? CCount { get; set; }
        public int? TodayAvaiableGolds { get; set; }
        public decimal? Golds { get; set; }
        public string Uuid { get; set; }
        public string Level { get; set; }
        public int? MonthlyTradeCount { get; set; }
        public string TradePwd { get; set; }
        public string Alipay { get; set; }

        public string AlipayUid { get; set; }
    }
}
