using System;
using System.Collections.Generic;
using System.Text;
using Yoyo.Entity.Enums;

namespace Yoyo.Entity.Models
{
    public class CityCashDividend
    {
        public Int64 Id { get; set; }
        public String CityNo { get; set; }
        public DividendType DividendType { get; set; }
        public Decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DividendState State { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public String Remark { get; set; }
    }
}
