using System;
using System.Collections.Generic;
using System.Text;

namespace Yoyo.IServices.Models
{
    /// <summary>
    /// 哔哔
    /// </summary>
    public class FraudUsers
    {
        /// <summary>
        /// 会员编号
        /// </summary>
        public List<Int64> Uids { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public List<String> Mobiles { get; set; }
    }
}
