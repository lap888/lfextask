namespace Yoyo.IServices.Models
{
    public class LFModel
    {
        /// <summary>
        /// 卖方id
        /// </summary>
        /// <value></value>
        public int sId { get; set; }
        /// <summary>
        /// 买方id
        /// </summary>
        /// <value></value>
        public int bId { get; set; }
        /// <summary>
        /// 卖方金额
        /// </summary>
        /// <value></value>
        public decimal sBalance { get; set; }
        /// <summary>
        /// 买方金额
        /// </summary>
        /// <value></value>
        public decimal bBalance { get; set; }
    }
}