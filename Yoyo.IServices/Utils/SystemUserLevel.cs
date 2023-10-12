using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IServices.Utils
{
    public class SystemUserLevel
    {
        /// <summary>
        /// 用户等级
        /// </summary>
        public string Level { get; set; }
        /// <summary>
        /// 要求贡献值
        /// </summary>
        public decimal Claim { get; set; }
        /// <summary>
        /// 购买糖果赠送果皮比例
        /// </summary>
        /// <remarks>
        /// 【公式:  果皮 = 糖果 * BuyRate】
        /// </remarks>
        public decimal BuyRate { get; set; }
        /// <summary>
        /// 出售糖果的手续费比例
        /// </summary>
        /// <remarks>
        /// 1、此值需要大于1
        /// 2、公式： 手续费 = 糖果 * (SellRate -1)
        /// </remarks>
        public decimal SellRate { get; set; }
        /// <summary>
        /// 兑换任务赠送果皮比例
        /// </summary>
        /// <remarks>
        /// 以任务赠送值为基数
        /// 公式：果皮=任务赠送值 * ExchangeRate
        /// </remarks>
        public decimal ExchangeRate { get; set; }
    }
}
