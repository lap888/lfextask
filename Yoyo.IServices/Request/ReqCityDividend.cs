using System;
using System.Collections.Generic;
using System.Text;
using Yoyo.Entity.Enums;

namespace Yoyo.IServices.Request
{
    /// <summary>
    /// 城市分红模型
    /// </summary>
    public class ReqCityDividend
    {
        /// <summary>
        /// 城市编号
        /// </summary>
        public String CityNo { get; set; }

        /// <summary>
        /// 分红金额
        /// </summary>
        public Decimal Amount { get; set; }

        /// <summary>
        /// 分红类型
        /// </summary>
        public DividendType Type { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public String[] Desc { get; set; }
    }
}
