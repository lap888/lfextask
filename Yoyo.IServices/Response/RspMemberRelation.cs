using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IServices.Response
{
    /// <summary>
    /// 会员拓扑关系
    /// </summary>
    public class RspMemberRelation
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
        /// 拓扑关系数组字符串
        /// </summary>
        public string TopologyString { get; set; }
        /// <summary>
        /// 拓扑关系
        /// </summary>
        public List<long> Topology { get; set; } = new List<long>();
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
