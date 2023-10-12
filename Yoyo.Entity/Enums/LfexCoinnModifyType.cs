using System.ComponentModel;

namespace Yoyo.Entity.Enums
{
    /// <summary>
    /// 账户变更类型
    /// </summary>
    public enum LfexCoinnModifyType
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Description("")]
        ALL = 0,
        /// <summary>
        /// 现金充值
        /// </summary>
        [Description("钱包充值 单号:{0}")]
        CASH_RECHARGE = 1,
        /// <summary>
        /// 钱包提现
        /// </summary>
        [Description("钱包提现 单号:{0}")]
        CASH_WITH_DRAW = 2,

        [Description("购买{0}个{1}奖励{2}个LF")]
        BuyCoinRewardCoin = 3,

        [Description("哟哟吧:{0}\r\n转入:{1}个糖果")]
        Yyb_MoveCoin_Tome = 24,
        [Description("LFEX提{0}个糖果,手续费{1}个糖果")]
        MoveCoin_To_Yyb = 25,
        [Description("{0}号矿机挖矿收入{1}LF")]
        Lf_Coin_Day_In = 26,
        [Description("下级{0}挖矿奖励{1}LF")]
        Lf_Xia_Ji_Give = 27,
        [Description("修复{0}号矿机消耗{1}LF")]
        Lf_Repair_Minning = 28,
        [Description("出售{0}个{1}手续费{2}")]
        Lf_Sell_Coin = 29,
        [Description("购买{0}个{1}")]
        Lf_buy_Coin = 30,
        [Description("购买{0}个{1}减{2}个USDT")]
        Lf_buy_Coin_Sub_Usdt = 31,
        [Description("卖掉{0}个{1}加{2}个USDT")]
        Lf_Sell_Coin_Add_Usdt = 32,
        [Description("订单号{0}的交易手续费{1}个USDT")]
        Lf_Sell_Sys_Fee = 33,
        [Description("修改邀请码{0}")]
        Lf_Modify_Code = 34,
    }
}