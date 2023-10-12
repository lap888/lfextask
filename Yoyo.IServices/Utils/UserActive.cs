using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IServices.Utils
{
    /// <summary>
    /// 用户活跃
    /// </summary>
    public class UserActive
    {
        /// <summary>
        /// 会员编号
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 极光设备标识
        /// </summary>
        public string JPushId { get; set; }
        /// <summary>
        /// 最后活跃时间
        /// </summary>
        public DateTime ActiveTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
