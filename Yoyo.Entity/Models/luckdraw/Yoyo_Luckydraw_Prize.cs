using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Yoyo.Entity.Models.luckdraw
{
    /// <summary>
    /// 夺宝奖品
    /// </summary>
    [Table("yoyo_luckydraw_prize")]
    public class Yoyo_Luckydraw_Prize
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 几等奖
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 奖品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 奖品描述
        /// </summary>
        public string Mark { get; set; }

        /// <summary>
        /// 奖品图片
        /// </summary>
        public string Pic { get; set; }

        /// <summary>
        /// 暂留
        /// </summary>
        public string StatusDesc { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreatedTime { get; set; }
        
    }
}
