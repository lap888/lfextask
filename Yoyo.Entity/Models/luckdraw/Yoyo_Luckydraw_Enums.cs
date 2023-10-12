using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.Entity.Models.luckdraw
{
    public enum RoundStatus
    {
        /// <summary>
        /// 开启中
        /// </summary>
        Rounding, 
        /// <summary>
        /// 已开奖
        /// </summary>
        Ending,
        /// <summary>
        /// 待开奖
        /// </summary>
        Waiting,
    }
    public enum WinnerType 
    {
        Default,Random
    }
}
