using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IServices.Request
{
    /// <summary>
    /// 消息订阅团队信息模型
    /// </summary>
    public class ReqTeamSubscribeInfo
    {
        /// <summary>
        /// 会员ID
        /// </summary>
        public long MemberId { get; set; }
        /// <summary>
        /// 推荐人ID【使用注册信道，此项必填】
        /// </summary>
        public long ParentId { get; set; }
        /// <summary>
        /// 任务等级
        /// </summary>
        public int TaskLevel { get; set; }
        /// <summary>
        /// 给予活跃贡献值
        /// </summary>
        public decimal Devote { get; set; }
        /// <summary>
        /// 是否续期任务
        /// </summary>
        public bool RenewTask { get; set; } = false;
    }
}
