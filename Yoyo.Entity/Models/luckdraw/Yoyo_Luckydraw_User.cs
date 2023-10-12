using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Yoyo.Entity.Models.luckdraw
{
    [Table("yoyo_luckydraw_user")]
    public class Yoyo_Luckydraw_User
    {
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 轮次Id
        /// </summary>
        public int RoundId { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 用户投入数量
        /// </summary>
        public int CandyCount { get; set; }
        /// <summary>
        /// 是否中间
        /// </summary>
        public bool IsWin { get; set; }
        /// <summary>
        /// 奖品Id
        /// </summary>
        public int PrizeId { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedTime { get; set; }

        public Yoyo_Luckydraw_Round Yoyo_Luckydraw_Round { get; set; }
        public User User { get; set; }
    }
}
