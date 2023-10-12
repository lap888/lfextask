using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yoyo.IPlugins
{
    /// <summary>
    /// 微信公众号
    /// </summary>
    public interface IWeChatPlugin
    {
        /// <summary>
        /// Code换OpenId
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        Task<String> GetOpenId(String Code);

        /// <summary>
        /// 发送魔板消息
        /// </summary>
        /// <param name="OpenId"></param>
        /// <param name="TempId"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        //Task SendTempMsg(String OpenId, String TempId, params String[] param);

    }
}
