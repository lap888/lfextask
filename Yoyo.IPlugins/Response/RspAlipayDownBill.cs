using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Yoyo.IPlugins.Response
{
    /// <summary>
    /// 对账单下载
    /// </summary>
    public class RspAlipayDownBill : Utils.AlipayResponse
    {
        /// <summary>
        /// 对账单下载地址
        /// </summary>
        [JsonProperty("bill_download_url")]
        public String BillDownUrl { get; set; }
    }
}
