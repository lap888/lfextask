using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Yoyo.Entity.Utils
{
    /// <summary>
    /// 用户实名认证状态
    /// </summary>
    public enum MemberAuthStatus
    {
        /// <summary>
        /// 未认证
        /// </summary>
        [Description("未认证")]
        UN_CERTIFIED = 0,
        /// <summary>
        /// 已提交人工
        /// </summary>
        [Description("已提交人工审核")]
        SUBMITTED = 1,
        /// <summary>
        /// 已认证
        /// </summary>
        [Description("已认证")]
        CERTIFIED = 2,
        /// <summary>
        /// 已拒绝
        /// </summary>
        [Description("已拒绝")]
        REJECTED = 3
    }
}
