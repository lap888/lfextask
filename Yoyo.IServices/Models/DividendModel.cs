using System;
using System.Collections.Generic;
using System.Text;
using Yoyo.Entity.Enums;

namespace Yoyo.IServices.Models
{
    public class DividendModel
    {
        /// <summary>
        /// 任务记录编号
        /// </summary>
        public Int64 RecordId { get; set; }

        /// <summary>
        /// 分红类型
        /// </summary>
        public DividendType DividendType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public String Remark { get; set; }
    }
}
