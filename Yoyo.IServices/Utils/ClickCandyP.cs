using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IServices.Utils
{
    public class ClickCandyP
    {
        /// <summary>
        /// 广告单日给予最多果皮
        /// </summary>
        public Decimal AdMax { get; set; }
        /// <summary>
        /// 用户单日果皮上限
        /// </summary>
        public Decimal UserMax { get; set; }
        /// <summary>
        /// 单次点击给予最下数量
        /// </summary>
        public Int32 OneMin { get; set; }
        /// <summary>
        /// 单次点击给予最多数量
        /// </summary>
        public Int32 OneMax { get; set; }
        /// <summary>
        /// 单次点击单位倍数
        /// </summary>
        public Decimal OneUnit { get; set; }
    }
}
