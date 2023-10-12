using System;
using System.Collections.Generic;

namespace Yoyo.Entity.Models
{
    public partial class MemberDevote
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime DevoteDate { get; set; }
        public decimal Devote { get; set; }
    }
}
