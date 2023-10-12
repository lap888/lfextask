using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IServices.Utils
{
    public enum TaskType
    {
        /// <summary>
        /// 用户实名
        /// </summary>
        USER_AUTH = 0,
        /// <summary>
        /// 广告分享
        /// </summary>
        SHARE_AD = 1,
        /// <summary>
        /// 出售糖果
        /// </summary>
        SELLER = 2,
        /// <summary>
        /// 购买糖果
        /// </summary>
        BUYER = 3,
        /// <summary>
        /// 玩游戏
        /// </summary>
        GAME = 4,
        /// <summary>
        /// 哟帮
        /// </summary>
        YOBON = 5,
        /// <summary>
        /// 抽奖
        /// </summary>
        LUCK_DRAW = 6,
        /// <summary>
        /// 视频观看
        /// </summary>
        WATCH_VEDIO = 7
    }
}
