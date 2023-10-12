using System;
using System.Collections.Generic;

namespace Yoyo.Entity.Models
{
    /// <summary>
    /// 用户关系
    /// </summary>
    public partial class MemberRelation
    {
        /// <summary>
        /// 会员ID
        /// </summary>
        public long MemberId { get; set; }
        /// <summary>
        /// 父级ID
        /// </summary>
        public long ParentId { get; set; }
        /// <summary>
        /// 关系层级
        /// </summary>
        public int RelationLevel { get; set; }
        /// <summary>
        /// 拓扑关系
        /// </summary>
        public string Topology { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
