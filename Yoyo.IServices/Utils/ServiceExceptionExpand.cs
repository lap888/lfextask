using System;

namespace Yoyo.IServices.Utils
{
    /// <summary>
    /// 服务异常拓展
    /// </summary>
    public static class ServiceExceptionExpand
    {
        /// <summary>
        /// 抛出系统服务错误
        /// </summary>
        /// <param name="code">错误码</param>
        public static void Throw(this Utils.ServiceCode code)
        {
            throw new ServiceException(code);
        }
        /// <summary>
        /// 抛出系统服务错误
        /// </summary>
        /// <param name="code">错误码</param>
        /// <param name="ex">异常信息</param>
        public static void Throw(this Utils.ServiceCode code, Exception ex)
        {
            Core.SystemLog.Error(ex);
            throw new ServiceException(code, ex);
        }
    }
}
