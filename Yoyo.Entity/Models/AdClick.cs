using System;
using System.Collections.Generic;

namespace Yoyo.Entity.Models
{
    public partial class AdClick
    {
        public long Id { get; set; }
        public int AdId { get; set; }
        public long UserId { get; set; }
        public decimal CandyP { get; set; }
        public string ClickId { get; set; }
        public DateTime ClickDate { get; set; }
        public DateTime ClickTime { get; set; }
    }
}
