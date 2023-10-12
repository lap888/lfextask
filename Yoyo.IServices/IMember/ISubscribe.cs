using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yoyo.IServices.IMember
{
    public interface ISubscribe
    {
        /// <summary>
        /// 消息订阅用户注册事件
        /// </summary>
        /// <param name="Msg">消息主体</param>
        /// <returns></returns>
        Task SubscribeMemberRegist(String Msg);

        /// <summary>
        /// 消息订阅用户认证事件
        /// </summary>
        /// <param name="Msg">消息主体</param>
        /// <returns></returns>
        Task SubscribeMemberCertified(String Msg);

        /// <summary>
        /// 消息订阅任务操作
        /// </summary>
        /// <param name="Msg">消息主体</param>
        /// <returns></returns>
        Task SubscribeTaskAction(String Msg);

        /// <summary>
        /// 消息订阅广告点击
        /// </summary>
        /// <param name="Msg">消息主体</param>
        /// <returns></returns>
        Task SubscribeClickAd(String Msg);

        /// <summary>
        /// 消息订阅，每日任务
        /// </summary>
        /// <param name="Msg"></param>
        /// <returns></returns>
        Task SubscribeTask(String Msg);

        /// <summary>
        /// 消息订阅，用户活跃
        /// </summary>
        /// <param name="Msg"></param>
        /// <returns></returns>
        Task SubscribeActive(String Msg);

        /// <summary>
        /// 系统收单并付款
        /// </summary>
        /// <param name="Msg"></param>
        /// <returns></returns>
        Task SubscribeTradeSystem(String Msg);

        /// <summary>
        /// 用户提现
        /// </summary>
        /// <param name="Msg"></param>
        /// <returns></returns>
        Task SubscribeWalletWithdraw(String Msg);

        /// <summary>
        /// 幸运宝箱
        /// </summary>
        /// <param name="Msg"></param>
        /// <returns></returns>
        Task BoxDividend(String Msg);

        /// <summary>
        /// 城市合伙人分红
        /// </summary>
        /// <param name="Msg"></param>
        /// <returns></returns>
        Task SubscribeCityDividend(String Msg);
        //LF变动更新更新用户团队LF持有量
        Task SubLfChange(string Mes);

        //LF变动更新更新用户团队LF持有量
        Task SubLfChangeSignle(string Mes);
    }
}
