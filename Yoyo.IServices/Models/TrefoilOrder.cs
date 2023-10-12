using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IServices.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class TrefoilOrder
    {
        /// <summary>
        /// 本地订单编号
        /// </summary>
        public long OrderNo { get; set; }
        /// <summary>
        /// 团长ID
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 团长佣金比例
        /// </summary>
        public long UserRate { get; set; }
        /// <summary>
        /// 团长佣金
        /// </summary>
        public long UserCommission { get; set; }
        /// <summary>
        /// 渠道ID
        /// </summary>
        public long ChannelId { get; set; }
        /// <summary>
        /// 渠道佣金比例
        /// </summary>
        public long ChannelRate { get; set; }
        /// <summary>
        /// 渠道佣金
        /// </summary>
        public long ChannelCommission { get; set; }
        /// <summary>
        /// 平台比例
        /// </summary>
        public long PlatformRate { get; set; }
        /// <summary>
        /// 平台佣金
        /// </summary>
        public long PlatformCommission { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public Int32 OrderStatus { get; set; }
        /// <summary>
        /// 联盟类型
        /// </summary>
        public Int32 UnionType { get; set; }
        /// <summary>
        /// 联盟订单号
        /// </summary>
        public string UnionNo { get; set; }
        /// <summary>
        /// 推广位
        /// </summary>
        public string UnionPid { get; set; }
        /// <summary>
        /// 自定义参数
        /// </summary>
        public string UnionCustom { get; set; }
        /// <summary>
        /// 商品ID
        /// </summary>
        public long GoodsId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }
        /// <summary>
        /// 商品主图
        /// </summary>
        public string GoodsImage { get; set; }
        /// <summary>
        /// 商品价格
        /// </summary>
        public long GoodsPrice { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        public int GoodsQuantity { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public long OrderAmount { get; set; }
        /// <summary>
        /// 实际金额
        /// </summary>
        public long ActualAmount { get; set; }
        /// <summary>
        /// 佣金比例
        /// </summary>
        public long CommissionRate { get; set; }
        /// <summary>
        /// 佣金金额
        /// </summary>
        public long CommissionAmount { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 变更时间
        /// </summary>
        public DateTime? ModifyTime { get; set; }
        /// <summary>
        /// 结算时间
        /// </summary>
        public DateTime? SettleTime { get; set; }
        /// <summary>
        /// 推送状态 0 未推送，1已推送
        /// </summary>
        public int NotifyStatus { get; set; }
    }
}
