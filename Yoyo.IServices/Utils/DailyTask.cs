using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IServices.Utils
{
    public class DailyTask
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Int64 UserId { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public TaskType TaskType { get; set; }
        /// <summary>
        /// 进度
        /// </summary>
        public Int32 CarryOut { get; set; }
        /// <summary>
        /// 给予活跃贡献值
        /// </summary>
        public Decimal Devote { get; set; }
    }
}
