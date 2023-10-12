using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.Entity.Enums
{
    /// <summary>
    /// 分红状态
    /// </summary>
    public enum DividendState
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 正常
        /// </summary>
        Normal = 1,

        /// <summary>
        /// 已分红
        /// </summary>
        Dividends = 2,

        /// <summary>
        /// 未获取地理位置
        /// </summary>
        NotCity = 3,
    }
}
