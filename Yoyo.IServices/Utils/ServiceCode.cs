using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Yoyo.IServices.Utils
{
    /// <summary>
    /// 服务代码
    /// </summary>
    public enum ServiceCode
    {
        /// <summary>
        /// 操作失败
        /// </summary>
        [Description("操作失败")]
        FAIL = -1,
        /// <summary>
        /// 操作成功
        /// </summary>
        [Description("操作成功")]
        SUCCESS = 0,

        #region 用户服务错误代码10系列
        /// <summary>
        /// 用户拓扑关系不存在
        /// </summary>
        [Description("用户拓扑关系不存在")]
        USER_RELATION_NOT_EXIST = 1020,
        /// <summary>
        /// 用户拓扑关系已存在
        /// </summary>
        [Description("用户拓扑关系已存在")]
        USER_RELATION_ALREADY_EXIST=1021,
        /// <summary>
        /// 用户推荐人拓扑不存在
        /// </summary>
        [Description("用户推荐人拓扑不存在")]
        USER_PARENT_RELATION_NOT_EXIST=1022,
        /// <summary>
        /// 设置用户拓扑失败
        /// </summary>
        [Description("设置用户拓扑失败")]
        USER_SET_RELATION_FAIL =1023,
        #endregion
    }
}
