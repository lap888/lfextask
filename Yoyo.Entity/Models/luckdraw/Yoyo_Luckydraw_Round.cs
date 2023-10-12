using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;
using System.Text;

namespace Yoyo.Entity.Models.luckdraw
{
    [Table("yoyo_luckydraw_round")]
    public class Yoyo_Luckydraw_Round
    {
        [Key]
        public int Id { get; set; }
        public int Level { get; set; }
        /// <summary>
        /// 该轮夺宝满多少开奖
        /// </summary>
        public int NeedRoundNumber { get; set; }
        /// <summary>
        /// 当前轮 积累了多少
        /// </summary>
        public int CurrentRoundNumber { get; set; }
        /// <summary>
        /// 该轮夺宝状态 开启中、已结束、待开奖
        /// </summary>
        [ConcurrencyCheck]
        public RoundStatus Status { get; set; }
        /// <summary>
        /// 最后一次时间
        /// </summary>
        public DateTime UpdatedTime { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedTime { get; set; }
        /// <summary>
        /// 开奖时间
        /// </summary>
        public DateTime? OpenTime { get; set; }
        /// <summary>
        /// 是的自动开启下一轮
        /// </summary>
        public bool AutoNext { get; set; }
        /// <summary>
        /// 中奖用户类型
        /// </summary>
        public WinnerType WinnerType { get; set; }
        /// <summary>
        /// 推迟多长时间再开奖
        /// </summary>
        public int DelayHour { get; set; }
        public int PrizeId { get; set; }
        /// <summary>
        /// 最大投入多少
        /// </summary>
        public int MaxNumber { get; set; }
        public Yoyo_Luckydraw_Prize Yoyo_Luckydraw_Prize { get; set; }
    }
}
