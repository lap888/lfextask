using System;
using System.Collections.Generic;

namespace Yoyo.Entity.Models
{
    /// <summary>
    /// 用户信息扩展表
    /// </summary>
    public partial class UserExt
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        public ulong Id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 团队星级
        /// </summary>
        public int TeamStart { get; set; }
        /// <summary>
        /// 团队人数
        /// </summary>
        public int TeamCount { get; set; }
        /// <summary>
        /// 直推人数
        /// </summary>
        public int AuthCount { get; set; }
        /// <summary>
        /// 团队果核
        /// </summary>
        public int TeamCandyH { get; set; }
        /// <summary>
        /// 大区果核
        /// </summary>
        public int BigCandyH { get; set; }
        /// <summary>
        /// 小区果核
        /// </summary>
        public int LittleCandyH { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
