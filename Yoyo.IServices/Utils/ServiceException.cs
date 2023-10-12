using System;
using Yoyo.Core.Expand;

namespace Yoyo.IServices.Utils
{
    public class ServiceException : Exception
    {
        public ServiceCode Code;
        public ServiceException()
        {
            this.Code = ServiceCode.FAIL;
        }

        public ServiceException(ServiceCode code) : base(code.GetDescription())
        {
            this.Code = code;
        }

        public ServiceException(ServiceCode code, Exception ex) : base(code.GetDescription(), ex)
        {
            this.Code = code;
        }
    }
}
