using System;
using System.Collections.Generic;

namespace Yoyo.Entity.Models
{
    public partial class MemberDuplicate
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 当日收购量
        /// </summary>
        public decimal Duplicate { get; set; }
    }
}
