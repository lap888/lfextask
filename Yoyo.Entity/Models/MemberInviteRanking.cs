using System;
using System.Collections.Generic;

namespace Yoyo.Entity.Models
{
    public partial class MemberInviteRanking
    {
        public long Id { get; set; }
        public int Phase { get; set; }
        public long UserId { get; set; }
        public int InviteTotal { get; set; }
        public int InviteToday { get; set; }
        public DateTime InviteDate { get; set; }
    }
}
