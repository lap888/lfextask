using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.Entity.Models
{
    /// <summary>
    /// 商城订单
    /// </summary>
    public partial class MallOrder
    {
        /// <summary>
        /// 编号
        /// </summary>
        public Int64 Id { get; set; }

        /// <summary>
        /// 会员编号
        /// </summary>
        public Int64 UserId { get; set; }

        /// <summary>
        /// 联盟订单编号
        /// </summary>
        public String UnionNo { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public Int32 UnionType { get; set; }

        /// <summary>
        /// 联盟推广位ID
        /// </summary>
        public String UnionPid { get; set; }

        /// <summary>
        /// 自定义参数
        /// </summary>
        public String UnionCustom { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public Int64 GoodsId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public String GoodsName { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary>
        public String GoodsImage { get; set; }

        /// <summary>
        /// 商品单价
        /// </summary>
        public Decimal GoodsPrice { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public Int32 GoodsQuantity { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public Decimal OrderAmount { get; set; }

        /// <summary>
        /// 佣金金额
        /// </summary>
        public Decimal Commission { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public Int32 OrderStatus { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public String Remark { get; set; }
    }
}
