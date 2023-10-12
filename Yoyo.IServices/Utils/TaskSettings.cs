using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IServices.Utils
{
    /// <summary>
    /// 任务配置
    /// </summary>
    public class TaskSettings
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 任务等级
        /// </summary>
        public int TaskLevel { get; set; }
        /// <summary>
        /// 所需糖果
        /// </summary>
        public Decimal CandyIn { get; set; }
        /// <summary>
        /// 团队任务果核
        /// </summary>
        public decimal TeamCandyH { get; set; }
    }
}
