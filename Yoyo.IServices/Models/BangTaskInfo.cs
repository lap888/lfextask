using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IServices.Models
{
    public class BangTaskInfo
    {
        /// <summary>
        /// 任务编号
        /// </summary>
        public Int64 TaskId { get; set; }
        /// <summary>
        /// 发布人
        /// </summary>
        public Int64 Publisher { get; set; }
        /// <summary>
        /// 做任务人编
        /// </summary>
        public Int64 UserId { get; set; }
        /// <summary>
        /// 奖金类型
        /// </summary>
        public Int32 RewardType { get; set; }
        /// <summary>
        /// 任务单价
        /// </summary>
        public Decimal UnitPrice { get; set; }
        /// <summary>
        /// 手续费比率
        /// </summary>
        public Decimal FeeRate { get; set; }
    }
}
