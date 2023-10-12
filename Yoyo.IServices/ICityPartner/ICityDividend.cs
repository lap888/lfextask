using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yoyo.IServices.Models;
using Yoyo.IServices.Request;

namespace Yoyo.IServices.ICityPartner
{
    public interface ICityDividend
    {
        /// <summary>
        /// 哟帮城市分红
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task YoBangDividend(DividendModel model);

        /// <summary>
        /// 广告城市分红
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task VideoDividend(DividendModel model);

        /// <summary>
        /// 游戏城市分红
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task ShandwDividend(DividendModel model);

        /// <summary>
        /// 商城城市分红
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task MallDividend(DividendModel model);

        /// <summary>
        /// 现金分红
        /// </summary>
        /// <param name="dividend"></param>
        /// <returns></returns>
        Task CashDividend(ReqCityDividend dividend);

        /// <summary>
        /// 糖果分红
        /// </summary>
        /// <param name="dividend"></param>
        /// <returns></returns>
        Task CandyDividend(ReqCityDividend dividend);
    }
}
